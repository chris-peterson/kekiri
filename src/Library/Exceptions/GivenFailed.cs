using System;
using System.Reflection;

namespace Kekiri.Exceptions
{
    public class GivenFailed : ScenarioTestException
    {
        public GivenFailed(ScenarioTest test, MethodBase stepMethod, Exception innerException) :
            base(test, string.Format("'{0}' failed", stepMethod.Name), innerException)
        {
        }
    }
}