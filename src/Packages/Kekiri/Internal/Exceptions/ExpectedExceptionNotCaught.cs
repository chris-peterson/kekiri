using System;

namespace Kekiri.Internal.Exceptions;

class ExpectedExceptionNotCaught : ScenarioException
{
    public ExpectedExceptionNotCaught(ScenarioBase scenario, Exception exception) :
        base(scenario, $"Expected {exception.GetType().Name}, missing Catch<{exception.GetType().Name}>()", exception)
    {
    }
}
