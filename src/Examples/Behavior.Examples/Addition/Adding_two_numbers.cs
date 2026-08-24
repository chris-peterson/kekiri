using AwesomeAssertions;
using Behavior;

namespace Behavior.Examples.Addition;

/// <summary>
/// Several scenarios in one fixture, sharing its steps and its fields. Each one runs against a
/// fresh instance.
/// </summary>
public class Adding_two_numbers : Scenarios
{
    Calculator _calculator;

    [Scenario]
    public void Adding_50_and_70()
    {
        Given(a_calculator)
            .And(the_user_enters_50)
            .And(the_user_enters_70);
        When(adding);
        Then(the_result_is_120);
    }

    [Scenario]
    public void Adding_nothing_to_nothing()
    {
        Given(a_calculator);
        When(adding);
        Then(the_result_is, 0m);
    }

    [Scenario]
    [Example(2, 3, 5)]
    [Example(10, 90, 100)]
    [Example(-1, 1, 0)]
    public void Adding_any_two_numbers(decimal first, decimal second, decimal expected)
    {
        Given(a_calculator)
            .And(the_operands_are, first, second);
        When(adding);
        Then(the_result_is, expected);
    }

    void a_calculator() => _calculator = new Calculator();

    void the_user_enters_50() => _calculator.Operand1 = 50;

    void the_user_enters_70() => _calculator.Operand2 = 70;

    void the_operands_are(decimal first, decimal second)
    {
        _calculator.Operand1 = first;
        _calculator.Operand2 = second;
    }

    void adding() => _calculator.Add();

    void the_result_is(decimal expected) => _calculator.Result.Should().Be(expected);

    void the_result_is_120() => _calculator.Result.Should().Be(120m);
}
