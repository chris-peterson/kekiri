using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kekiri.Impl
{
    class StepClassInvoker : IStepInvoker
    {
        readonly Type _stepClass;
        readonly IExceptionHandler _exceptionHandler;

        public StepClassInvoker(StepType stepType, Type stepClass, KeyValuePair<string,object>[] supportedParameters, IExceptionHandler exceptionHandler)
        {
            if (!typeof(Step).GetTypeInfo().IsAssignableFrom(stepClass))
                throw new ArgumentException("The stepClass must inherit from Step", nameof(stepClass));
            _stepClass = stepClass;
            _exceptionHandler = exceptionHandler;
            Type = stepType;
            Name = new StepName(Type, _stepClass.Name, supportedParameters);
            Parameters = _stepClass.GetTypeInfo().GetConstructors().Single().BindParameters(supportedParameters);
        }

        public StepType Type { get; }

        public StepName Name { get; }

        public KeyValuePair<string, object>[] Parameters { get; } 

        public bool ExceptionExpected { get; set; }

        public string SourceDescription => _stepClass.FullName;

        public Task InvokeAsync(ScenarioBase scenario)
        {
           return Step.InstanceFor(scenario, _stepClass, Parameters, _exceptionHandler).ExecuteAsync();
        }
    }
}