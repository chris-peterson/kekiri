using System;
using System.Linq;

namespace Kekiri.Impl
{
    public class StepClassInvoker : IStepInvoker
    {
        private readonly Type _stepClass;

        public StepClassInvoker(Type stepClass) : this(GetStepType(stepClass), stepClass) { }

        public StepClassInvoker(StepType stepType, Type stepClass)
        {
            if (!typeof(Step).IsAssignableFrom(stepClass))
                throw new ArgumentException("The stepClass must inherit from Step", "stepClass");
            _stepClass = stepClass;
            Type = stepType;
            Name = new StepName(Type, _stepClass.Name);
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

        private static StepType GetStepType(Type stepClass)
        {
            var stepName = Enum.GetNames(typeof (StepType))
                .FirstOrDefault(name => stepClass.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase));
            if(stepName == null)
                throw new InvalidOperationException("The step class name should begin with one of " + String.Join(",", Enum.GetNames(typeof(StepType))));
            return (StepType)Enum.Parse(typeof (StepType), stepName, ignoreCase: true);
        }
    }
}