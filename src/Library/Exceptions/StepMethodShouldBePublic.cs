using System;
using System.Reflection;

namespace Kekiri.Exceptions
{
    public class StepMethodShouldBePublic : ScenarioTestException
    {
        public StepMethodShouldBePublic(Type scenarioType, MethodBase nonPublicGiven)
            : base(scenarioType, string.Format("'{0}' is not public", nonPublicGiven.Name))
        {
        }
    }
}