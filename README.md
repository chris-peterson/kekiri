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

Kekiri targets `netstandard2.0`.  To get started, be sure to have the latest [dotnet core](https://www.microsoft.com/net/core) tools.

### Select Test Runner

#### Xunit (recommended)

`PM> Install-Package Kekiri.Xunit`

#### NUnit

`PM> Install-Package Kekiri.NUnit`

### IoC Integration (optional)

#### Autofac

`PM> Install-Package Kekiri.IoC.Autofac`

Be sure to call `AutofacBootstrapper.Initialize()` before your tests run.

#### IServiceProvider

`PM> Install-Package Kekiri.IoC.ServiceProvider`

Be sure to call `ServiceProviderBootstrapper.Initialize(…)` before your tests run.

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
            Then(the_screen_displays_a_result_of_120);
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
            Assert.AreEqual(120m, _calculator.Result);
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
        public Divide_by_zero()
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
        public Subtracting_two_numbers(double operand1, double operand2, double expectedResult)
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
            Assert.AreEqual(expected, _calculator.Result);
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

## Acknowledgements

Kekiri uses and is influenced by the following open source projects:

* [Xunit](<https://xunit.github.io>)
* [NUnit](<http://nunit.org>)
* [Autofac](<https://github.com/autofac/Autofac>)
* [xrepo](<https://github.com/andyalm/xrepo>)
* [pickles](<https://github.com/picklesdoc/pickles#pickles>)
