using System;

namespace Kekiri.Exceptions
{
    internal class ScenarioStepMethodsShouldNotHaveParameters : ScenarioTestException
    {
        public ScenarioStepMethodsShouldNotHaveParameters(Type scenarioType, string message) :
            base(scenarioType, message)
        {
        }
    }
}