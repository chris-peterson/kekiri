namespace Behavior.Tests.Samples.Arithmetic;

[Category("fast")]
[Tag("Owner", "payments")]
public class Tagged_scenario : Scenarios
{
    [Scenario]
    [Category("smoke")]
    public void Carries_its_tags()
    {
        Given(nothing);
        When(nothing);
        Then(nothing);
    }

    void nothing()
    {
    }
}
