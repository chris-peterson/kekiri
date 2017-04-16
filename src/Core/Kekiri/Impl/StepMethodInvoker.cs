using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kekiri.Impl
{
    class StepMethodInvoker : IStepInvoker
    {
        protected MethodBase Method { get; }

        public bool ExceptionExpected { get; set; }
        
        public StepName Name { get; }

        public StepType Type { get; }

        public string SourceDescription { get; }

        public KeyValuePair<string, object>[] Parameters { get; } 

        public StepMethodInvoker(StepType stepType, MethodBase method, KeyValuePair<string, object>[] supportedParameters = null)
        {
            Method = method;
            Type = stepType;
            Parameters = method.BindParameters(supportedParameters);
            Name = new StepName(Type, method.Name, supportedParameters);
            SourceDescription = $"{method.DeclaringType?.FullName}.{method.Name}";
        }

        public virtual void Invoke(ScenarioBase scenario)
        {
            Method.Invoke(Method.IsStatic ? null : scenario, Parameters.Select(p => p.Value).ToArray());
        }
    }
}