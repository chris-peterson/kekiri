using System;

namespace Behavior.Internal.Exceptions;

class GivenFailed : ScenarioException
{
    public GivenFailed(ScenarioBase scenario, string stepName, Exception innerException) :
        base(scenario, stepName, $"'{stepName}' failed", innerException)
    {
    }
}
