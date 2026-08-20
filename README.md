# Overview

A .NET framework that supports writing low-ceremony BDD tests using Gherkin language.

Kekiri honors the conventions of the Gherkin
[cucumber language](https://cucumber.io/docs/gherkin/reference/).

## Status

[![build](https://github.com/chris-peterson/kekiri/actions/workflows/ci.yml/badge.svg)](https://github.com/chris-peterson/kekiri/actions/workflows/ci.yml)

Package | Latest Release |
:-------- | :------------ |
Kekiri | [![NuGet version](https://img.shields.io/nuget/dt/Kekiri.svg)](https://www.nuget.org/packages/kekiri)
Kekiri.IoC.Autofac | [![NuGet version](https://img.shields.io/nuget/dt/Kekiri.IoC.Autofac.svg)](https://www.nuget.org/packages/kekiri.ioc.autofac)
Kekiri.IoC.ServiceProvider | [![NuGet version](https://img.shields.io/nuget/dt/Kekiri.IoC.ServiceProvider.svg)](https://www.nuget.org/packages/kekiri.ioc.ServiceProvider)
Kekiri.Xunit | [![NuGet version](https://img.shields.io/nuget/dt/Kekiri.Xunit.svg)](https://www.nuget.org/packages/kekiri.xunit)
Kekiri.NUnit | [![NuGet version](https://img.shields.io/nuget/dt/Kekiri.NUnit.svg)](https://www.nuget.org/packages/kekiri.nunit)

## Setup

Kekiri targets `net8.0`.  To get started, be sure to have the latest [dotnet](https://www.microsoft.com/net/core) tools.

### Select Test Runner

#### xUnit (recommended)

`PM> Install-Package Kekiri.Xunit`

Built on [xUnit.net v3](https://xunit.net/docs/getting-started/v3/migration), so the test project is
a self-executing application (`<OutputType>Exe</OutputType>`).

#### NUnit

`PM> Install-Package Kekiri.NUnit`

### IoC Integration (optional)

#### Autofac

`PM> Install-Package Kekiri.IoC.Autofac`

#### IServiceProvider

`PM> Install-Package Kekiri.IoC.ServiceProvider`

#### Bootstrapping

The container is built once per assembly. Under xUnit that's an assembly fixture — scenario classes
need no `[Collection]` attribute and no shared base class:

```csharp
[assembly: AssemblyFixture(typeof(Bootstrap))]

public class Bootstrap
{
    public Bootstrap() => AutofacBootstrapper.Initialize();
}
```

Under NUnit, use a `[SetUpFixture]` with `[OneTimeSetUp]`. See
[setup](https://chris-peterson.github.io/kekiri/#/setup) for both in full.

## Why Kekiri

Unlike other BDD frameworks that impose process overhead (management of feature files, custom tooling, etc) Kekiri allows developers to write BDD scenarios just as quickly and easily as they would a "plain old" test.

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

If we were to run this test (even though it fails) we get a nice Cucumber-style feature output:

```plaintext
        Scenario: Adding two numbers
        Given a calculator
            And the user enters 50
            And next the user enters 70
        When adding
        Then the result is 120
```

### Add the implementation

```c#
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
            Assert.Equal(120m, _calculator.Result);
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

Kekiri supports both Pascal case conventions (e.g. `WhenDoingTheThing`) as it does
underscore convention (e.g. `When_doing_the_thing`).

---

## Scenario Output

Kekiri supports outputing the cucumber text.
The output settings are controlled via the `KEKIRI_OUTPUT` environment variable.

Example:

```ps1
   $env:KEKIRI_OUTPUT='console,files'
```

### Output to Console

To output to the console, ensure that `KEKIRI_OUTPUT` contains `console`.

### Output to Files

To output to .feature files in the test execution directory, ensure that `KEKIRI_OUTPUT` contains `files`.

The name of the feature file is based on the containing namespace of the scenario.
For example, if `Adding_two_numbers` was defined in `UnitTests.Features.Addition.Adding_two_numbers`, the output would be written to `Addition.feature`.

---

## Wiki

More detailed documentation can be found on the [wiki](<https://github.com/chris-peterson/kekiri/wiki>).

## Other common use cases

### Expected Exceptions

```c#
    class Divide_by_zero : Scenarios
    {
        readonly Calculator _calculator = new Calculator();

        [Scenario]
        public void Divide_by_zero()
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

```c#
    public class Subtracting_two_numbers : Scenarios
    {
        readonly Calculator _calculator = new Calculator();

        [Example(12, 5, 7)]
        [Example(20, 5, 15)]
        [ScenarioOutline]
        public void Subtracting_two_numbers(double operand1, double operand2, double expectedResult)
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
            Assert.Equal(expected, _calculator.Result);
        }
    }
```

```plaintext
        Given the user enters 12
          And the user enters 5
        When subtracting
        Then the result is 7
```

Note: step method parameter names can be used as substitution macros by mentioning them in CAPS.

For more advanced topics, check out the [wiki](https://github.com/chris-peterson/kekiri/wiki).

## Native AOT

Not supported, and not currently worth pursuing. Recorded here so it doesn't get re-investigated
per-project.

Three things block it independently, so fixing any one of them changes nothing on its own:

1. **The untyped `Context` is `dynamic`.** Every `Context.Foo = ...` in a scenario compiles to a
   call site on the C# runtime binder, which needs runtime code generation. This is the documented way
   to write an untyped scenario, so removing it means removing a first-class feature (or reshaping
   `Context` into an indexer, which breaks every existing untyped scenario).
2. **`Kekiri.IoC.Autofac` is assembly scanning, by design.** It reads `DependencyContext.Default`,
   loads assemblies by name, and hands them to Autofac's `RegisterAssemblyTypes`.
   `DependencyContext.Default` is documented as returning null for an app published as a single file,
   and Autofac's scanning entry points carry `[RequiresUnreferencedCode]` because the trimmer cannot
   know which types to keep. Native AOT always trims, so the package cannot keep its behavior and be
   AOT-safe.
3. **xUnit v3 under AOT replaces reflection discovery with source generators.** It needs different
   packages (`xunit.v3.aot`), .NET 9 or later, and a source generator for anything extending its
   extensibility points. Kekiri is exactly such an extension: it ships custom discoverers, test cases,
   and runners.

Independently, `Moq` relies on `Reflection.Emit` through Castle DynamicProxy, so a test project using
mocks is out regardless of what Kekiri does.

An AOT-capable subset would be `Scenarios<TContext>` plus `Kekiri.IoC.ServiceProvider` without
`UseStartup`, on xUnit v3 — that is, giving up untyped scenarios and Autofac. If that combination ever
becomes worth supporting, start by setting `IsAotCompatible` on the libraries to get the real analyzer
output rather than working from this list.

Worth doing regardless of AOT, because they cost nothing and remove reflection from paths that don't
need it:

* `Kekiri.IoC.ServiceProvider`'s `UseStartup` reconstructs a generic call through
  `MakeGenericMethod`; capturing a delegate while the type argument is still known statically removes
  it, the same way `ConfigureTestContainer` already does.
* `ScenarioBase<TContext>` resolves its context through `MakeGenericMethod` on `Container.Resolve`.
  Constraining `TContext` to a reference type makes it a direct `Container.Resolve<TContext>()` call.
* Two binder call sites on the *typed* path (the `Context` overrides in `ScenarioBase<TContext>` and
  `Step<TContext>`) pull the runtime binder into scenarios that are otherwise fully typed. Casting
  from an `object` backing field removes them with no public API change.
* `FeatureFileReportTarget` keeps a `Dictionary<string, dynamic>` where a `string` value would do.

None of the above has been checked against an actual `PublishAot` run; it comes from reading the code
and the AOT and xUnit documentation.

## Acknowledgements

Kekiri uses and is influenced by the following open source projects:

* [xUnit.net](<https://xunit.net>)
* [NUnit](<http://nunit.org>)
* [Autofac](<https://github.com/autofac/Autofac>)
* [xrepo](<https://github.com/andyalm/xrepo>)
* [pickles](<https://github.com/picklesdoc/pickles#pickles>)
