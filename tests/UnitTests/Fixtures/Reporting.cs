namespace Kekiri.Tests.Fixtures;

public class Reporting : TestFixture
{
    public void Every_step_kind()
    {
        Given(a_calculator).And(a_second_operand);
        When(when);
        Then(it_works).But(But_nothing_is_saved).But(nothing_is_sent).And(But_this_one_was_named_but);
    }

    void nothing_is_sent()
    {
    }

    void But_this_one_was_named_but()
    {
    }

    void a_calculator()
    {
    }

    void a_second_operand()
    {
    }

    /// <summary>Only the keyword, so the When reads as the fixture's name.</summary>
    void when()
    {
    }

    void it_works()
    {
    }

    void But_nothing_is_saved()
    {
    }
}
