using System;
using NUnit.Framework;

namespace Kekiri
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ThenAttribute : TestAttribute, IStepAttribute
    {
        public StepType StepType
        {
            get { return StepType.Then; }
        }
    }
}