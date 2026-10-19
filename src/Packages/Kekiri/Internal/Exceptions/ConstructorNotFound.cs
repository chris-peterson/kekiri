using System;

namespace Kekiri.Internal.Exceptions;

class ConstructorNotFound : ScenarioException
{
    public ConstructorNotFound(ScenarioBase scenario, string message) : base(scenario, message)
    {
    }
}
