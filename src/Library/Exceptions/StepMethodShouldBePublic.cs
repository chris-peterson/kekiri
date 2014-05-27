using System;
using System.Reflection;

namespace Kekiri.Exceptions
{
    internal class StepMethodShouldBePublic : ScenarioTestException
    {
        public StepMethodShouldBePublic(Type type, MethodBase nonPublicGiven)
            : base(type, string.Format("'{0}' is not public", nonPublicGiven.Name))
        {
        }
    }
}