namespace Behavior.Tests.Samples.Broken;

public class Missing_examples : Scenarios
{
    [Scenario]
    public void Takes_an_argument_with_no_example(int value)
    {
        Given(nothing);
        When(nothing);
        Then(nothing);
    }

    void nothing()
    {
    }
}
