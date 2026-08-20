using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Kekiri.Mtp
{
    /// <summary>
    /// One discovered scenario: a [Scenario] method on a Scenarios class. Discovery is the same
    /// reflection Kekiri's xUnit discoverers do, just not filtered through a host framework's
    /// test-case model first.
    /// </summary>
    sealed class Scenario
    {
        readonly Type _class;
        readonly MethodInfo _method;

        Scenario(Type @class, MethodInfo method)
        {
            _class = @class;
            _method = method;
        }

        public static IEnumerable<Scenario> DiscoverIn(Assembly assembly) =>
            assembly.GetTypes()
                .Where(t => !t.IsAbstract && typeof(ScenarioBase).IsAssignableFrom(t))
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.GetCustomAttribute<ScenarioAttribute>() != null)
                    .Select(m => new Scenario(t, m)))
                .OrderBy(s => s.FeatureName)
                .ThenBy(s => s.Title);

        /// <summary>Stable across runs and machines: the platform uses it to target one test.</summary>
        public string Uid => $"{_class.FullName}.{_method.Name}";

        public string FeatureUid => $"feature:{FeatureName}";

        /// <summary>
        /// Kekiri already derives a feature from the containing namespace for .feature output; the
        /// same grouping is what a reader wants in a navigation tree.
        /// </summary>
        public string FeatureName => _class.Namespace?.Split('.').Last() ?? _class.Name;

        public string Title => Humanize(_method.Name);

        /// <summary>
        /// What an IDE needs to put "go to source" on the scenario. Null when the compiler didn't
        /// supply a path, which the caller has to tolerate.
        /// </summary>
        public TestFileLocationProperty FileLocation
        {
            get
            {
                var attribute = _method.GetCustomAttribute<ScenarioAttribute>();

                return string.IsNullOrEmpty(attribute?.SourceFilePath)
                    ? null
                    : new TestFileLocationProperty(
                        attribute.SourceFilePath,
                        new LinePositionSpan(
                            new LinePosition(attribute.SourceLineNumber, 0),
                            new LinePosition(attribute.SourceLineNumber, 0)));
            }
        }

        public TestMethodIdentifierProperty MethodIdentifier => new TestMethodIdentifierProperty(
            _class.Assembly.FullName,
            _class.Namespace ?? string.Empty,
            _class.Name,
            _method.Name,
            // Method arity. The docs' signature omits it; the assembly requires it.
            _method.IsGenericMethodDefinition ? _method.GetGenericArguments().Length : 0,
            _method.GetParameters().Select(p => p.ParameterType.FullName).ToArray(),
            _method.ReturnType.FullName);

        public async Task<Outcome> RunAsync()
        {
            object instance = null;
            try
            {
                instance = Activator.CreateInstance(_class);

                // The method only records the Given/When/Then chain; running it is the second step.
                _method.Invoke(instance, null);
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

        static IReadOnlyList<string> StepsOf(object instance) =>
            instance switch
            {
                Scenarios s => s.Captured.Steps,
                _ => instance?.GetType()
                        .GetField("Captured", BindingFlags.Instance | BindingFlags.NonPublic)
                        ?.GetValue(instance) is Internal.CapturingReportTarget captured
                            ? captured.Steps
                            : new string[0],
            };

        static string Humanize(string methodName) => methodName.Replace('_', ' ');

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
}
