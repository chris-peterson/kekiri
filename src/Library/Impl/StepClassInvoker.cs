using System;
using System.Linq;

namespace Kekiri.Impl
{
    public class StepClassInvoker : IStepInvoker
    {
        private readonly Type _stepClass;

        public StepClassInvoker(Type stepClass)
        {
            if(!typeof(Step).IsAssignableFrom(stepClass))
                throw new ArgumentException("The stepClass must inherit from Step", "stepClass");
            _stepClass = stepClass;
            Type = GetStepType();
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

        public void Invoke(ScenarioTest test)
        {
            Step.InstanceFor(test, _stepClass).Execute();
        }

        private StepType GetStepType()
        {
            var stepName = Enum.GetNames(typeof (StepType))
                .FirstOrDefault(name => _stepClass.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase));
            if(stepName == null)
                throw new InvalidOperationException("The step class name should begin with one of " + String.Join(",", Enum.GetNames(typeof(StepType))));
            return (StepType)Enum.Parse(typeof (StepType), stepName, ignoreCase: true);
        }
    }
}