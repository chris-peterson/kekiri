using System;
using System.Collections.Generic;
using System.Linq;

namespace Kekiri.Impl
{
    internal class StepClassInvoker : IStepInvoker
    {
        private readonly Type _stepClass;

        public StepClassInvoker(StepType stepType, Type stepClass, IDictionary<string,object> substitutionParameters)
        {
            if (!typeof(Step).IsAssignableFrom(stepClass))
                throw new ArgumentException("The stepClass must inherit from Step", "stepClass");
            _stepClass = stepClass;
            Type = stepType;
            Name = new StepName(Type, _stepClass.Name, substitutionParameters);
        }

        public StepType Type { get; private set; }

        public StepName Name { get; private set; }

        public bool SuppressOutput
        {
            get { return false; }
        }

        public bool ExceptionExpected
        {
            get { return _stepClass.HasAttribute<ThrowsAttribute>(); }
        }

        public string SourceDescription
        {
            get { return _stepClass.FullName; }
        }

        public void Invoke(object test)
        {
            var contextContainer = test as IContextAccessor;
            if(contextContainer == null)
                throw new InvalidOperationException("The test must implement IContextContainer");
            Step.InstanceFor(contextContainer, _stepClass).Execute();
        }
    }
}