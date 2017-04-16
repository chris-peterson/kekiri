using System;

namespace Kekiri.Impl.Exceptions
{
    class GivenFailed : ScenarioException
    {
        public GivenFailed(ScenarioBase scenario, string stepName, Exception innerException) :
            base(scenario, $"'{stepName}' failed", innerException)
        {
        }
    }
}