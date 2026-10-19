using System;

namespace Kekiri.Internal.Exceptions;

class NoExceptionThrown : ScenarioException
{
    public NoExceptionThrown(
        ScenarioBase scenario) :
        base(scenario, "Expected an exception, but nothing was thrown", null)
    {
    }

    public NoExceptionThrown(
        ScenarioBase scenario,
        Type expectedExceptionType) :
        base(scenario, $"Expected {expectedExceptionType.Name}, but nothing was thrown", null)
    {
    }
}
