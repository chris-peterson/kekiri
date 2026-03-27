# Examples

## Basic Calculator

### Start with the test

```csharp
class Calculator_tests : Scenarios
{
    [Scenario]
    public void Adding_two_numbers()
    {
        Given(a_calculator)
           .And(the_user_enters_50)
           .And(the_user_enters_70);
        When(adding);
        Then(the_result_is_120);
    }

    void a_calculator() {}
    void the_user_enters_50() {}
    void the_user_enters_70() {}
    void adding() { throw new NotImplementedException(); }
    void the_result_is_120() {}
}
```

Even though the test fails, you get Cucumber-style feature output:

```text
Scenario: Adding two numbers
Given a calculator
    And the user enters 50
    And next the user enters 70
When adding
Then the result is 120
```

### Add the implementation

```csharp
class Adding_two_numbers : Scenarios
{
    Calculator _calculator;

    [Scenario]
    public void Adding_two_numbers()
    {
        Given(a_calculator)
           .And(the_user_enters_50)
           .And(the_user_enters_70);
        When(adding);
        Then(the_screen_displays_a_result_of_120);
    }

    void a_calculator() { _calculator = new Calculator(); }
    void the_user_enters_50() { _calculator.Operand1 = 50; }
    void the_user_enters_70() { _calculator.Operand2 = 70; }
    void adding() { _calculator.Add(); }
    void the_result_is_120() { Assert.AreEqual(120m, _calculator.Result); }
}
```

## Expected Exceptions

```csharp
class Divide_by_zero : Scenarios
{
    readonly Calculator _calculator = new Calculator();

    [Scenario]
    public Divide_by_zero()
    {
        Given(a_denominator_of_0);
        When(dividing).Throws();
        Then(an_exception_is_raised);
    }

    void a_denominator_of_0() { _calculator.Operand2 = 0; }
    void dividing() { _calculator.Divide(); }
    void an_exception_is_raised() { Catch<DivideByZeroException>(); }
}
```

Use `Throws()` to indicate that throwing an exception is expected behavior. In one or more `Then` methods, the thrown exception type must be caught using `Catch<>`.

## Scenario Outlines (Tabular Tests)

```csharp
public class Subtracting_two_numbers : Scenarios
{
    readonly Calculator _calculator = new Calculator();

    [Example(12, 5, 7)]
    [Example(20, 5, 15)]
    [ScenarioOutline]
    public Subtracting_two_numbers(double operand1, double operand2, double expectedResult)
    {
        Given(the_user_enters_OPERAND1, operand1)
            .And(the_user_enters_OPERAND2, operand2);
        When(subtracting);
        Then(the_result_is_EXPECTED, expectedResult);
    }

    void the_user_enters_OPERAND1(double operand1) { _calculator.Operand1 = operand1; }
    void the_user_enters_OPERAND2(double operand2) { _calculator.Operand2 = operand2; }
    void subtracting() { _calculator.Subtract(); }
    void the_result_is_EXPECTED(double expected) { Assert.AreEqual(expected, _calculator.Result); }
}
```

```text
Given the user enters 12
  And the user enters 5
When subtracting
Then the result is 7
```

Step method parameter names can be used as substitution macros by mentioning them in CAPS.

For more advanced topics, check out the [wiki](https://github.com/chris-peterson/kekiri/wiki).
