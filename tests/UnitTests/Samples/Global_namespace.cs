// A fixture outside any namespace takes its feature name from the class.
public class Without_a_namespace : Kekiri.TestFixture
{
    [Kekiri.Scenario]
    public void Still_has_a_feature()
    {
        When(nothing);
        Then(nothing);
    }

    void nothing()
    {
    }
}
