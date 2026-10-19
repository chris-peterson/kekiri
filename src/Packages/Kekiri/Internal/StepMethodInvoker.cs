using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Kekiri.Internal;

class StepMethodInvoker : IStepInvoker
{
    protected MethodBase Method { get; }

    readonly object _target;

    public bool ExceptionExpected { get; set; }
    
    public StepName Name { get; }

    public StepType Type { get; }

    public string SourceDescription { get; }

    public KeyValuePair<string, object>[] Parameters { get; } 

    /// <param name="target">
    /// The delegate's own target: a lambda's method lives on a compiler-generated closure rather
    /// than on the fixture.
    /// </param>
    /// <param name="expression">The source text the caller passed, which names a lambda step.</param>
    public StepMethodInvoker(StepType stepType, MethodBase method, object target, KeyValuePair<string, object>[] supportedParameters = null, string expression = null)
    {
        Method = method;
        _target = target;
        Type = stepType;
        Parameters = method.BindParameters(supportedParameters);
        Name = expression != null && LambdaText.IsCompilerGenerated(method.Name)
            ? StepName.Literal(Type, LambdaText.Body(expression))
            : new StepName(Type, method.Name, supportedParameters);
        SourceDescription = $"{method.DeclaringType?.FullName}.{method.Name}";
    }

    public virtual Task InvokeAsync(ScenarioBase scenario)
    {
        var returnValue = Method.Invoke(_target,Parameters.Select(p => p.Value).ToArray());

        if (returnValue is Task returnTask)
        {
            return returnTask;
        }
        else
        {
            return Task.CompletedTask;
        }
    }
}
