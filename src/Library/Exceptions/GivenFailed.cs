using System;

namespace Kekiri.Exceptions
{
    internal class GivenFailed : ScenarioTestException
    {
        public GivenFailed(object test, string stepName, Exception innerException) :
            base(test, string.Format("'{0}' failed", stepName), innerException)
        {
        }
    }
}