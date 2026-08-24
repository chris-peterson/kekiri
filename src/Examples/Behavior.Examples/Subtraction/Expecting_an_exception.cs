using System;
using AwesomeAssertions;
using Behavior;

namespace Behavior.Examples.Subtraction;

/// <summary>
/// When(...).Throws() moves the failure into the Thens, where Catch names the type expected.
/// </summary>
public class Expecting_an_exception : Scenarios
{
    Calculator _calculator;

    [Scenario]
    public void An_expected_exception_is_caught()
    {
        Given(a_calculator);
        When(dividing_by_zero).Throws();
        Then(a_divide_by_zero_is_reported);
    }

    void a_calculator() => _calculator = new Calculator();

    void dividing_by_zero() =>
        _calculator.Result = _calculator.Operand1 / _calculator.Operand2;

    void a_divide_by_zero_is_reported() =>
        Catch<DivideByZeroException>().Message.Should().Be("Attempted to divide by zero.");
}
