using System;

namespace Kekiri.Impl.Exceptions
{
    class ThenFailed : ScenarioException
    {
        public ThenFailed(object test, string stepName, Exception innerException) :
            base(test, $"'{stepName}' failed", innerException)
        {
        }
    }
}