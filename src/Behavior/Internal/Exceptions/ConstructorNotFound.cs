using System;

namespace Behavior.Internal.Exceptions;

class ConstructorNotFound : ScenarioException
{
    public ConstructorNotFound(ScenarioBase scenario, string message) : base(scenario, message)
    {
    }
}
