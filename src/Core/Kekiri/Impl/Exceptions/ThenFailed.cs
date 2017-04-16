using System;

namespace Kekiri.Impl.Exceptions
{
    class ThenFailed : ScenarioException
    {
        public ThenFailed(ScenarioBase scenario, string stepName, Exception innerException) :
            base(scenario, $"'{stepName}' failed", innerException)
        {
        }
    }
}