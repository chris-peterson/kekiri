using System;

namespace Behavior.Tests.Samples.Broken;

public class Failing_then : Scenarios
{
    [Scenario]
    public void The_last_step_throws()
    {
        Given(nothing);
        When(nothing);
        Then(a_step_that_throws);
    }

    void nothing()
    {
    }

    void a_step_that_throws() => throw new InvalidOperationException("the cause");
}
