# Kekiri
A .NET framework that supports writing low-ceremony BDD tests using Gherkin language.

Kekiri honors the conventions of the [cucumber language] (https://github.com/cucumber/cucumber/wiki/Feature-Introduction)

## Setup
Select your Test running framework and install the appropriate package.

### NUnit
`PM> Install-Package Kekiri.TestRunner.NUnit`  

## Why Kekiri?
Unlike other BDD frameworks that impose process overhead (management of feature files, custom tooling, etc) Kekiri allows developers to write BDD scenarios just as quickly and easily as they would a "plain old" test.

The resulting scenario fixtures are concise, highly portable, and adhere to [Act, Arrange, and Assert](http://www.arrangeactassert.com/why-and-what-is-arrange-act-assert/).

## Example
For this **Scenario**, we will be implementing a basic calculator.

### Start with the test

```c#
    class Adding_two_numbers : Scenario 
    {
        Adding_two_numbers()
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

        Scenario: Adding two numbers
        Given a calculator
            And the user enters 50
            And next the user enters 70
        When adding
        Then the result is 120

### Add the implementation
```c#
    class Adding_two_numbers : Scenario 
    {    
        Calculator _calculator;

        Adding_two_numbers() 
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
Kekiri supports both Pascal case conventions (e.g. `WhenDoingTheThing`) just as it does
underscore convention (e.g. `When_doing_the_thing`).

---

## .feature file output
In addition to outputing to the console, Kekiri generates .feature files in the test execution directory.  The name of the feature file is based on the containing namespace of the scenario.
For example, if `Adding_two_numbers` was defined in `UnitTests.Features.Addition.Adding_two_numbers`, the output would be written to `Addition.feature`.

---

## Wiki
[More info available here](https://github.com/chris-peterson/Kekiri/wiki)

## Other common use cases

### Expected Exceptions
```c#
    class Divide_by_zero : Scenario 
    {
        readonly Calculator _calculator = new Calculator();

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

Notice, here we've used the `Throws()` method to inform the **Scenario** that throwing an
exception is the expected behavior.  In 1 or more `Then` methods, the thrown type of exception must
be caught (using the templated method `Catch<>`).

### Data-driven
```c#
    [Example(12, 5, 7)]
    [Example(20, 5, 15)]
    public class Subtracting_two_numbers : Scenario 
    {
        readonly Calculator _calculator = new Calculator();

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

        Given the user enters 12
          And the user enters 5
        When subtracting
        Then the result is 7

Note: step method parameter names can be used as substitution macros by mentioning them in CAPS.

For more advanced topics, check out the [wiki](https://github.com/chris-peterson/Kekiri/wiki).

## Contributing

1. Fork it
2. Create your feature branch `git checkout -b my-new-feature`
3. Commit your changes `git commit -am 'Added some feature'`
4. Push to the branch `git push origin my-new-feature`
5. Create new Pull Request


## Acknowledgements
Kekiri uses and is influenced by the following open source projects:
* http://nunit.org/
* https://code.google.com/p/autofac/
* https://github.com/dennisdoomen/FluentAssertions
* https://github.com/andyalm/xrepo
* https://github.com/picklesdoc/pickles
