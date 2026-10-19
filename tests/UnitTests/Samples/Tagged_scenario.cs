namespace Kekiri.Tests.Samples.Arithmetic;

[Category("fast")]
[Tag("Owner", "payments")]
public class Tagged_scenario : TestFixture
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
