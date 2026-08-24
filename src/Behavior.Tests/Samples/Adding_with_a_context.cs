using System;

namespace Behavior.Tests.Samples.Typed;

public class Adding_with_a_context : Scenarios<Running_total>
{
    [Scenario]
    public void The_context_carries_state_between_steps()
    {
        Given(a_starting_value, 40);
        When(adding, 2);
        Then(the_value_is, 42);
    }

    void a_starting_value(int value) => Context.Value = value;

    void adding(int amount) => Context.Value += amount;

    void the_value_is(int expected)
    {
        if (Context.Value != expected)
        {
            throw new Exception($"expected {expected}, was {Context.Value}");
        }
    }
}
