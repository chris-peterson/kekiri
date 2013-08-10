using System;

namespace Kekiri
{
    [AttributeUsage(AttributeTargets.Method)]
    public class WhenAttribute : Attribute, IStepAttribute
    {
        public StepType StepType
        {
            get { return StepType.When; }
        }
    }
}