using AwesomeAssertions;
using Behavior;

namespace Behavior.Examples.Addition;

/// <summary>
/// A typed context is state the steps share without fields on the fixture, which is what makes
/// a step reusable across fixtures.
/// </summary>
public class Adding_with_a_typed_context : Scenarios<Sum>
{
    [Scenario]
    public void Adding_1_and_2()
    {
        Given(a_number, 1)
            .And(another_number, 2);
        When(adding_them_up);
        Then(the_sum_is, 3);
    }

    [Scenario]
    [Example(1, 2, 3)]
    [Example(2, 3, 5)]
    [Example(-1, 1, 0)]
    public void Adding_any_two_numbers(int first, int second, int expected)
    {
        Given(a_number, first)
            .And(another_number, second);
        When(adding_them_up);
        Then(the_sum_is, expected);
    }

    void a_number(int value) => Context.First = value;

    void another_number(int value) => Context.Second = value;

    void adding_them_up() => Context.Total = Context.First + Context.Second;

    void the_sum_is(int expected) => Context.Total.Should().Be(expected);
}
