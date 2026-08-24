using AwesomeAssertions;
using Behavior;

namespace Behavior.Examples.Subtraction;

/// <summary>
/// A second namespace, so the run has two features to group under. The category rides along as
/// test metadata, which is what --treenode-filter "/**[Category=fast]" resolves against.
/// </summary>
[Category("fast")]
public class Subtracting_two_numbers : Scenarios
{
    Calculator _calculator;

    [Scenario]
    public void Subtracting_5_from_12()
    {
        Given(a_calculator)
            .And(the_user_enters_12)
            .And(the_user_enters_5);
        When(subtracting);
        Then(the_result_is_7);
    }

    void a_calculator() => _calculator = new Calculator();

    void the_user_enters_12() => _calculator.Operand1 = 12;

    void the_user_enters_5() => _calculator.Operand2 = 5;

    void subtracting() => _calculator.Subtract();

    void the_result_is_7() => _calculator.Result.Should().Be(7m);
}
