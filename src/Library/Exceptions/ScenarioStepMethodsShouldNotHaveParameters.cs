using System;

namespace Kekiri.Exceptions
{
    public class ScenarioStepMethodsShouldNotHaveParameters : ScenarioTestException
    {
        public ScenarioStepMethodsShouldNotHaveParameters(Type scenarioType, string message) :
            base(scenarioType, message)
        {
        }
    }
}