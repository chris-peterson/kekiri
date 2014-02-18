using System;

namespace Kekiri.Exceptions
{
    public class GivenFailed : ScenarioTestException
    {
        public GivenFailed(ScenarioTest test, string stepName, Exception innerException) :
            base(test, string.Format("'{0}' failed", stepName), innerException)
        {
        }
    }
}