using System;

namespace Kekiri.Internal.Exceptions;

class WrongExceptionType : ScenarioException
{
    public WrongExceptionType(
        ScenarioBase scenario,
        Type expectedExceptionType,
        Exception innerException)
        : base(
            scenario,
            $"Expected {expectedExceptionType.Name}, but was {innerException.GetType().Name}",
            innerException)
    {
    }
}
