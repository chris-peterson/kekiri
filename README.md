# Overview

A .NET framework that supports writing low-ceremony BDD tests using Gherkin language.

Behavior honors the conventions of the Gherkin
[cucumber language](https://cucumber.io/docs/gherkin/reference/).

## Status

[![build](https://github.com/chris-peterson/kekiri/actions/workflows/ci.yml/badge.svg)](https://github.com/chris-peterson/kekiri/actions/workflows/ci.yml)

Package | Latest Release |
:-------- | :------------ |
Behavior | [![NuGet version](https://img.shields.io/nuget/dt/Behavior.svg)](https://www.nuget.org/packages/behavior)
Behavior.Autofac | [![NuGet version](https://img.shields.io/nuget/dt/Behavior.Autofac.svg)](https://www.nuget.org/packages/behavior.autofac)
Behavior.ServiceProvider | [![NuGet version](https://img.shields.io/nuget/dt/Behavior.ServiceProvider.svg)](https://www.nuget.org/packages/behavior.serviceprovider)

## Setup

Behavior targets `net8.0`.  To get started, be sure to have the latest [dotnet](https://www.microsoft.com/net/core) tools.

### Install

`PM> Install-Package Behavior`

`Behavior` discovers and runs scenarios directly on
[Microsoft.Testing.Platform](https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-intro),
so it is the only framework reference a scenario project needs. The project is a self-executing
application (`<OutputType>Exe</OutputType>`) and the entry point is generated. Assertions come from
whichever library you prefer; the examples use [AwesomeAssertions](https://awesomeassertions.org).

### IoC Integration (optional)

#### Autofac

`PM> Install-Package Behavior.Autofac`

#### IServiceProvider

`PM> Install-Package Behavior.ServiceProvider`

#### Bootstrapping

The container is built once per assembly, by any class implementing `IBeforeTestRun`:

```csharp
public class Bootstrap : IBeforeTestRun
{
    public void Setup() => AutofacBootstrapper.Initialize();
}
```

Scenario classes need no attribute and no shared base beyond `Scenarios`. See
[setup](https://chris-peterson.github.io/kekiri/#/setup) for the rest.

Behavior is what Kekiri became, under new package ids. Coming from `Kekiri`, `Kekiri.Xunit` or
`Kekiri.NUnit`? Scenario bodies carry over as they are — the project file, one namespace, and a few
attributes change. See [migrating](https://chris-peterson.github.io/kekiri/#/migrating).

## Why Behavior

Unlike other BDD frameworks that impose process overhead (management of feature files, custom tooling, etc) Behavior allows developers to write BDD scenarios just as quickly and easily as they would a "plain old" test.

The resulting scenario fixtures are concise, highly portable, and adhere to [Act, Arrange, and Assert](https://automationpanda.com/2020/07/07/arrange-act-assert-a-pattern-for-writing-good-tests/).

IoC is also a first-class citizen encouraging testing object interactions in collaboration rather than isolation.  More details [here](https://github.com/chris-peterson/kekiri/wiki/IoC-Support).

## Example

Implementing a basic calculator.

### Start with the test

```c#
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

If we were to run this test (even though it fails) we get a nice Cucumber-style feature output,
with `✗` on the step that failed and the reason beneath it:

```plaintext
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

```c#
    class Adding_two_numbers : Scenarios
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

        void a_calculator()
        {
            _calculator = new Calculator();
        }

        void the_user_enters_50()
        {
            _calculator.Operand1 = 50;
        }

        void the_user_enters_70()
        {
            _calculator.Operand2 = 70;
        }

        void adding()
        {
            _calculator.Add();
        }

        void the_result_is_120()
        {
            _calculator.Result.Should().Be(120m);
        }
    }

    class Calculator
    {
        public decimal Operand1 { get; set; }
        public decimal Operand2 { get; set; }

        public decimal Result { get; set; }

        public void Add() { Result = Operand1 + Operand2; }
    }
```

---

## Supported Naming Conventions

Behavior supports both Pascal case conventions (e.g. `WhenDoingTheThing`) as it does
underscore convention (e.g. `When_doing_the_thing`).

---

## Scenario Output

Behavior supports outputing the cucumber text.
The output settings are controlled via the `BEHAVIOR_OUTPUT` environment variable.

Example:

```ps1
   $env:BEHAVIOR_OUTPUT='console,files'
```

### Output to Console

To output to the console, ensure that `BEHAVIOR_OUTPUT` contains `console`.

### Output to Files

To output to .feature files in the test execution directory, ensure that `BEHAVIOR_OUTPUT` contains `files`.

The name of the feature file is based on the containing namespace of the scenario.
For example, if `Adding_two_numbers` was defined in `UnitTests.Features.Addition.Adding_two_numbers`, the output would be written to `Addition.feature`.

---

## Wiki

More detailed documentation can be found on the [wiki](<https://github.com/chris-peterson/kekiri/wiki>).

## Other common use cases

### Expected Exceptions

```c#
    class Expecting_an_exception : Scenarios
    {
        readonly Calculator _calculator = new Calculator();

        [Scenario]
        public void An_expected_exception_is_caught()
        {
            Given(a_denominator_of_0);
            When(dividing).Throws();
            Then(an_exception_is_raised);
        }

        void a_denominator_of_0()
        {
            _calculator.Operand2 = 0;
        }

        void dividing()
        {
            _calculator.Divide();
        }

        void an_exception_is_raised()
        {
            Catch<DivideByZeroException>();
        }
    }
```

Notice, here we've used the `Throws()` method to inform that throwing an
exception is the expected behavior.  In 1 or more `Then` methods, the thrown type of exception must
be caught (using the templated method `Catch<>`).

### Examples (aka tabular tests)

Give a scenario `[Example]` rows and it runs once per row, reported as one test per row:

```c#
    public class Subtracting_two_numbers : Scenarios
    {
        readonly Calculator _calculator = new Calculator();

        [Scenario]
        [Example(12, 5, 7)]
        [Example(20, 5, 15)]
        public void Subtracting_any_two_numbers(double operand1, double operand2, double expectedResult)
        {
            Given(the_user_enters_OPERAND1, operand1)
                .And(the_user_enters_OPERAND2, operand2);
            When(subtracting);
            Then(the_result_is_EXPECTED, expectedResult);
        }

        void the_user_enters_OPERAND1(double operand1)
        {
            _calculator.Operand1 = operand1;
        }

        void the_user_enters_OPERAND2(double operand2)
        {
            _calculator.Operand2 = operand2;
        }

        void subtracting()
        {
            _calculator.Subtract();
        }

        void the_result_is_EXPECTED(double expected)
        {
            _calculator.Result.Should().Be(expected);
        }
    }
```

```plaintext
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

Note: step method parameter names can be used as substitution macros by mentioning them in CAPS.

### Tags

A tag is Gherkin's `@tag`: a name/value pair on a scenario or on a whole fixture, which reports and
IDEs group and filter on. `[Category]` is the one every .NET runner already spells that way, and it
is shorthand for `[Tag("Category", …)]`:

```c#
    [Category("fast")]
    [Tag("Owner", "payments")]
    public class Subtracting_two_numbers : Scenarios
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

## Native AOT

Not supported, and not currently worth pursuing. Recorded here so it doesn't get re-investigated
per-project.

Three things block it independently, so fixing any one of them changes nothing on its own:

1. **The untyped `Context` is `dynamic`.** Every `Context.Foo = ...` in a scenario compiles to a
   call site on the C# runtime binder, which needs runtime code generation. This is the documented way
   to write an untyped scenario, so removing it means removing a first-class feature (or reshaping
   `Context` into an indexer, which breaks every existing untyped scenario).
2. **`Behavior.Autofac` is assembly scanning, by design.** It reads `DependencyContext.Default`,
   loads assemblies by name, and hands them to Autofac's `RegisterAssemblyTypes`.
   `DependencyContext.Default` is documented as returning null for an app published as a single file,
   and Autofac's scanning entry points carry `[RequiresUnreferencedCode]` because the trimmer cannot
   know which types to keep. Native AOT always trims, so the package cannot keep its behavior and be
   AOT-safe.
3. **Discovery and step invocation are reflection.** The runner enumerates the test assembly's
   types, reads attributes, and calls step methods through `MethodInfo.Invoke`. Under AOT the trimmer
   cannot know which of those to keep, so discovery would have to move to a source generator.

Independently, `Moq` relies on `Reflection.Emit` through Castle DynamicProxy, so a test project using
mocks is out regardless of what Behavior does.

An AOT-capable subset would be `Scenarios<TContext>` plus `Behavior.ServiceProvider` without
`UseStartup`, behind a source generator for discovery — that is, giving up untyped scenarios and
Autofac. If that combination ever becomes worth supporting, start by setting `IsAotCompatible` on the
libraries to get the real analyzer output rather than working from this list.

Worth doing regardless of AOT, because they cost nothing and remove reflection from paths that don't
need it:

* `Behavior.ServiceProvider`'s `UseStartup` reconstructs a generic call through
  `MakeGenericMethod`; capturing a delegate while the type argument is still known statically removes
  it, the same way `ConfigureTestContainer` already does.
* `ScenarioBase<TContext>` resolves its context through `MakeGenericMethod` on `Container.Resolve`.
  Constraining `TContext` to a reference type makes it a direct `Container.Resolve<TContext>()` call.
* Two binder call sites on the *typed* path (the `Context` overrides in `ScenarioBase<TContext>` and
  `Step<TContext>`) pull the runtime binder into scenarios that are otherwise fully typed. Casting
  from an `object` backing field removes them with no public API change.
* `FeatureFileReportTarget` keeps a `Dictionary<string, dynamic>` where a `string` value would do.

None of the above has been checked against an actual `PublishAot` run; it comes from reading the code
and the AOT documentation.

## References

Behavior runs on Microsoft.Testing.Platform:

* [Overview](<https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-intro>) — what the platform is and how a test project runs on it
* [Build a test framework](<https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-architecture-test-framework>) — the `ITestFramework` surface Behavior implements to own discovery and execution
* [Build extensions](<https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-architecture-extensions>) — the extension points the platform hands a framework
* [CLI options](<https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-cli-options>) — `--treenode-filter`, `--filter-uid`, and the rest of what a run accepts
