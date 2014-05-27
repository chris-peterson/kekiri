using System;

namespace Kekiri.Exceptions
{
    internal class ScenarioStepMethodsShouldNotHaveParameters : ScenarioTestException
    {
        public ScenarioStepMethodsShouldNotHaveParameters(Type type, string message) :
            base(type, message)
        {
        }
    }
}