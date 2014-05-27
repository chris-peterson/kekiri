using System;

namespace Kekiri.Exceptions
{
    internal class ThenFailed : ScenarioTestException
    {
        public ThenFailed(object test, string stepName, Exception innerException) :
            base(test, string.Format("'{0}' failed", stepName), innerException)
        {
        }
    }
}