# Examples

## Basic Calculator

### Start with the test

<!-- snippet: compile-only -->
```csharp
class Calculator_tests : TestFixture
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
    void adding() => throw new NotImplementedException();
    void the_result_is_120() {}
}
```

Even though the test fails, you get Cucumber-style feature output, with `✗` on the step that failed
and the reason beneath it:

```text
Feature: Calculator

  Scenario: Adding two numbers
    Given a calculator
      And the user enters 50
      And the user enters 70
  ✗ When adding
      The method or operation is not implemented.
    Then the result is 120
```

The feature name comes from the containing namespace.

### Add the implementation

```csharp
class Adding_two_numbers : TestFixture
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

    void a_calculator() => _calculator = new Calculator();
    void the_user_enters_50() => _calculator.Operand1 = 50;
    void the_user_enters_70() => _calculator.Operand2 = 70;
    void adding() => _calculator.Add();
    void the_result_is_120() => _calculator.Result.Should().Be(120m);
}
```

```text
Feature: Addition

  Scenario: Adding 50 and 70
    Given a calculator
      And the user enters 50
      And the user enters 70
    When adding
    Then the result is 120
    ✓ passed (22ms)
```

## Expected Exceptions

```csharp
class Expecting_an_exception : TestFixture
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
    void dividing_by_zero() => _calculator.Result = _calculator.Operand1 / _calculator.Operand2;
    void a_divide_by_zero_is_reported() => Catch<DivideByZeroException>();
}
```

Use `Throws()` to indicate that throwing an exception is expected behavior. In one or more `Then` methods, the thrown exception type must be caught using `Catch<>`.

## Scenario Outlines (Tabular Tests)

Give a scenario `[Example]` rows and it runs once per row, reported as one test per row:

```csharp
public class Subtracting_two_numbers : TestFixture
{
    readonly Calculator _calculator = new Calculator();

    [Scenario]
    [Example(12, 5, 7)]
    [Example(20, 5, 15)]
    public void Subtracting_any_two_numbers(decimal operand1, decimal operand2, decimal expected)
    {
        Given(the_user_enters_OPERAND1, operand1)
            .And(the_user_enters_OPERAND2, operand2);
        When(subtracting);
        Then(the_result_is_EXPECTED, expected);
    }

    void the_user_enters_OPERAND1(decimal operand1) => _calculator.Operand1 = operand1;
    void the_user_enters_OPERAND2(decimal operand2) => _calculator.Operand2 = operand2;
    void subtracting() => _calculator.Subtract();
    void the_result_is_EXPECTED(decimal expected) => _calculator.Result.Should().Be(expected);
}
```

```text
Feature: Subtraction

  Scenario: Subtracting any two numbers [12, 5, 7]
    Given the user enters 12
      And the user enters 5
    When subtracting
    Then the result is 7
    ✓ passed (12ms)

  Scenario: Subtracting any two numbers [20, 5, 15]
    Given the user enters 20
      And the user enters 5
    When subtracting
    Then the result is 15
    ✓ passed (1ms)
```

Step method parameter names can be used as substitution macros by mentioning them in CAPS.

## Lambda Steps

A step can be a lambda where a named method would say nothing more than its body. The Gherkin reads
the lambda's body, and an empty one reads as nothing, so a "nothing to do here" step stays out of
the report:

```csharp
public class Clearing_the_calculator : TestFixture
{
    readonly Calculator _calculator = new Calculator();

    [Scenario]
    public void Clearing_resets_the_result()
    {
        Given(() => _calculator.Result = 42);
        When(() => _calculator.Result = 0);
        Then(the_result_is_zero);
    }

    void the_result_is_zero() => _calculator.Result.Should().Be(0);
}
```

```text
  Scenario: Clearing resets the result
    Given _calculator.Result = 42
    When _calculator.Result = 0
    Then the result is zero
```

## Tags

A tag is Gherkin's `@tag`: a name/value pair on a scenario or on a whole fixture, which reports and
IDEs group and filter on. `[Category]` is the one every .NET runner already spells that way, and it
is shorthand for `[Tag("Category", …)]`:

<!-- snippet: skip -->
```csharp
[Category("fast")]
[Tag("Owner", "payments")]
public class Subtracting_two_numbers : TestFixture
```

Both are repeatable, and both apply to a single scenario as well as to a fixture. A fixture's tags
carry to every scenario in it.

Filter a run by any tag, or by the feature:

```bash
dotnet run --treenode-filter '/**[Category=fast]'
dotnet run --treenode-filter '/**[Owner=payments]'
dotnet run --treenode-filter '/**[Feature=Subtraction]'
```

For more advanced topics, check out the [wiki](https://github.com/chris-peterson/kekiri/wiki).
