using System;

namespace Kekiri
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GivenAttribute : Attribute, IStepAttribute
    {
        public StepType StepType
        {
            get { return StepType.Given; }
        }
    }
}