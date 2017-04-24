using System;

namespace Kekiri.Impl.Exceptions
{
    class ConstructorNotFound : ScenarioException
    {
        public ConstructorNotFound(ScenarioBase scenario, string message) : base(scenario, message)
        {
        }
    }
}