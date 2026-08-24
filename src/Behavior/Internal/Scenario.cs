using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Behavior;

/// <summary>
/// One discovered scenario: a [Scenario] method, or one of its [Example] rows. Discovery is
/// reflection over the test assembly, with no host framework's test-case model in between.
/// </summary>
sealed class Scenario
{
    readonly Type _class;
    readonly MethodInfo _method;
    readonly object[] _arguments;
    readonly int _exampleIndex;

    Scenario(Type @class, MethodInfo method, object[] arguments = null, int exampleIndex = -1)
    {
        _class = @class;
        _method = method;
        _arguments = arguments;
        _exampleIndex = exampleIndex;
    }

    public static IEnumerable<Scenario> DiscoverIn(Assembly assembly) =>
        assembly.GetTypes()
            .Where(t => !t.IsAbstract && typeof(ScenarioBase).IsAssignableFrom(t))
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .SelectMany(m => From(t, m)))
            .OrderBy(s => s.FeatureName)
            .ThenBy(s => s.ScenarioName)
            // Sorting on the title instead would alphabetize an outline's rows by their
            // rendered arguments, so the tree would disagree with the Examples table.
            .ThenBy(s => s._exampleIndex);

    static IEnumerable<Scenario> From(Type @class, MethodInfo method)
    {
        if (method.GetCustomAttribute<ScenarioAttribute>() == null)
        {
            yield break;
        }

        var examples = method.GetCustomAttributes<ExampleAttribute>().ToArray();

        if (examples.Length == 0)
        {
            // Parameters with no [Example] to fill them is a mistake worth seeing rather than a
            // test that vanishes from the count, so it is reported as one scenario that fails
            // when it runs.
            yield return method.GetParameters().Length == 0
                ? new Scenario(@class, method)
                : new Scenario(@class, method, new object[0], 0);

            yield break;
        }

        for (var i = 0; i < examples.Length; i++)
        {
            yield return new Scenario(@class, method, examples[i].Data, i);
        }
    }

    bool IsExample => _exampleIndex >= 0;

    /// <summary>Stable across runs and machines: the platform uses it to target one test.</summary>
    public string Uid => IsExample
        ? $"{_class.FullName}.{_method.Name}#{_exampleIndex}"
        : $"{_class.FullName}.{_method.Name}";

    public string FeatureUid => $"feature:{FeatureName}";

    /// <summary>
    /// Behavior already derives a feature from the containing namespace for .feature output; the
    /// same grouping is what a reader wants in a navigation tree.
    /// </summary>
    public string FeatureName =>
        Internal.Names.Sentence(_class.Namespace?.Split('.').Last() ?? _class.Name);

    public string ScenarioName => Internal.Names.Sentence(_method.Name);

    public string Title => IsExample ? $"{ScenarioName} [{Render(_arguments)}]" : ScenarioName;

    /// <summary>
    /// What an IDE needs to put "go to source" on the scenario. Null when the compiler didn't
    /// supply a path, which the caller has to tolerate.
    /// </summary>
    public TestFileLocationProperty FileLocation
    {
        get
        {
            var path = SourceFilePath;

            return string.IsNullOrEmpty(path)
                ? null
                : new TestFileLocationProperty(
                    path,
                    new LinePositionSpan(
                        new LinePosition(SourceLineNumber, 0),
                        new LinePosition(SourceLineNumber, 0)));
        }
    }

    string SourceFilePath => _method.GetCustomAttribute<ScenarioAttribute>()?.SourceFilePath;

    int SourceLineNumber => _method.GetCustomAttribute<ScenarioAttribute>()?.SourceLineNumber ?? 0;

    /// <summary>
    /// What an IDE builds its tree from, and it has to name a type that exists: substituting the
    /// feature here (an empty namespace, a type name with spaces in it) costs the whole tree,
    /// and an IDE reports that as no tests rather than as a bad identifier. Feature grouping
    /// therefore rides on the Feature trait, which is a value rather than an identifier.
    /// </summary>
    public TestMethodIdentifierProperty MethodIdentifier => new TestMethodIdentifierProperty(
        _class.Assembly.FullName,
        _class.Namespace ?? string.Empty,
        _class.Name,
        _method.Name,
        // Method arity. The docs' signature omits it; the assembly requires it.
        _method.IsGenericMethodDefinition ? _method.GetGenericArguments().Length : 0,
        _method.GetParameters().Select(p => p.ParameterType.FullName).ToArray(),
        _method.ReturnType.FullName);

    /// <summary>
    /// The path a TreeNodeFilter matches against, mirroring the identifier an IDE built its
    /// tree from so a filter written against what the reader sees resolves.
    /// </summary>
    public string NodePath =>
        $"/{_class.Assembly.GetName().Name}/{_class.Namespace}/{_class.Name}/{_method.Name}";

    /// <summary>
    /// The feature as test metadata. It reaches the tree this way rather than as a parent node,
    /// and it is what an IDE groups on.
    /// </summary>
    public TestMetadataProperty FeatureProperty => new TestMetadataProperty("Feature", FeatureName);

    /// <summary>
    /// The properties a filter's [name=value] expressions are resolved against. The feature is
    /// among them, so a filter can name the grouping the reader sees.
    /// </summary>
    public PropertyBag FilterProperties =>
        new PropertyBag(Tags.Prepend(FeatureProperty).Cast<IProperty>().ToArray());

    /// <summary>Tags from the method and its declaring fixture, method-level first.</summary>
    public IEnumerable<TestMetadataProperty> Tags =>
        _method.GetCustomAttributes<TagAttribute>()
            .Concat(_class.GetCustomAttributes<TagAttribute>(inherit: true))
            .Select(t => new TestMetadataProperty(t.Name, t.Value));

    public async Task<Outcome> RunAsync()
    {
        object instance = null;
        try
        {
            var arguments = BindArguments();

            instance = Activator.CreateInstance(_class);

            // The method only records the Given/When/Then chain; running it is the second step.
            _method.Invoke(instance, arguments);
            await ((ScenarioBase)instance).RunAsync();

            return new Outcome(null, StepsOf(instance));
        }
        catch (TargetInvocationException ex) when (ex.InnerException != null)
        {
            return new Outcome(ex.InnerException, StepsOf(instance));
        }
        catch (Exception ex)
        {
            return new Outcome(ex, StepsOf(instance));
        }
    }

    /// <summary>
    /// An attribute argument is stored as whatever type the literal had, so an [Example(2)] row
    /// against a decimal parameter arrives boxed as int and MethodInfo.Invoke rejects it —
    /// reflection does none of the widening the compiler would have done at a call site.
    /// </summary>
    object[] BindArguments()
    {
        if (!IsExample)
        {
            return null;
        }

        var parameters = _method.GetParameters();

        if (_arguments.Length != parameters.Length)
        {
            throw new InvalidOperationException(
                $"{_method.Name} takes {parameters.Length} argument(s), but the [Example] " +
                $"supplies {_arguments.Length}.");
        }

        return _arguments.Select((a, i) => Coerce(a, parameters[i].ParameterType)).ToArray();
    }

    static object Coerce(object value, Type target)
    {
        if (value == null || target.IsInstanceOfType(value))
        {
            return value;
        }

        var underlying = Nullable.GetUnderlyingType(target) ?? target;

        if (underlying.IsEnum)
        {
            return Enum.ToObject(underlying, value);
        }

        // Anything else is left alone so Invoke reports the mismatch, rather than a conversion
        // here inventing a value the caller didn't write.
        return value is IConvertible
            ? Convert.ChangeType(value, underlying, System.Globalization.CultureInfo.InvariantCulture)
            : value;
    }

    /// <summary>
    /// An example row as a reader would write it in an Examples table. Strings are quoted so an
    /// empty one and a null are distinguishable in the tree.
    /// </summary>
    static string Render(object[] arguments) =>
        arguments == null ? string.Empty : string.Join(", ", arguments.Select(Render));

    static string Render(object argument)
    {
        switch (argument)
        {
            case null:
                return "null";
            case string text:
                return $"\"{text}\"";
            case bool flag:
                return flag ? "true" : "false";
            case IEnumerable items:
                return $"[{string.Join(", ", items.Cast<object>().Select(Render))}]";
            default:
                return Convert.ToString(argument, System.Globalization.CultureInfo.InvariantCulture);
        }
    }

    static IReadOnlyList<string> StepsOf(object instance) =>
        instance is IRecordsSteps recorder ? recorder.Captured.Steps : new string[0];

    internal sealed class Outcome
    {
        public Outcome(Exception failure, IReadOnlyList<string> steps)
        {
            Failure = failure;
            Steps = steps ?? new string[0];
        }

        public Exception Failure { get; }

        public IReadOnlyList<string> Steps { get; }
    }
}
