using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kekiri.Exceptions;

namespace Kekiri.Impl
{
    internal class StepMethodInvoker : IStepInvoker
    {
        protected MethodBase Method { get; private set; }

        public bool ExceptionExpected { get; private set; }

        public StepName Name { get; private set; }

        public StepType Type { get; private set; }

        public string SourceDescription { get; private set; }

        public bool SuppressOutput { get; private set; }

        public KeyValuePair<string, object>[] Parameters { get; private set; } 

        public StepMethodInvoker(MethodBase method, KeyValuePair<string, object>[] supportedParameters = null)
            : this(method.AttributeOrDefault<IStepAttribute>().StepType, method, supportedParameters) { }

        public StepMethodInvoker(StepType stepType, MethodBase method, KeyValuePair<string, object>[] supportedParameters = null)
        {
            Method = method;
            Type = stepType;
            Parameters = GetParameters(method, supportedParameters);
            Name = new StepName(Type, method.Name, supportedParameters);
            SuppressOutput = method.SuppressOutputAttribute() != null;
            ExceptionExpected = method.AttributeOrDefault<ThrowsAttribute>() != null;
            SourceDescription = string.Format("{0}.{1}", method.DeclaringType.FullName, method.Name);
        }

        private KeyValuePair<string, object>[] GetParameters(MethodBase method, KeyValuePair<string, object>[] supportedParameters)
        {
            supportedParameters = supportedParameters ?? new KeyValuePair<string, object>[0];
            var methodParameters = method.GetParameters();
            return supportedParameters
                .Where(supportedParam => methodParameters.Any(p => p.Name.Equals(supportedParam.Key, StringComparison.OrdinalIgnoreCase)))
                .ToArray();
        }

        public virtual void Invoke(object test)
        {
            Method.Invoke(Method.IsStatic ? null : test, Parameters.Select(p => p.Value).ToArray());
        }
    }
}