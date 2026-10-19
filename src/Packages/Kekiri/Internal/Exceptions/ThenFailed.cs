using System;

namespace Kekiri.Internal.Exceptions;

class ThenFailed : ScenarioException
{
    public ThenFailed(ScenarioBase scenario, string stepName, Exception innerException) :
        base(scenario, stepName, $"'{stepName}' failed", innerException)
    {
    }
}
