# Kekiri
A .NET framework that supports writing low-ceremony BDD tests using Gherkin language.

Kekiri honors the conventions of the [cucumber language] (https://github.com/cucumber/cucumber/wiki/Feature-Introduction)

## Setup
`PM> Install-Package Kekiri`  
`C:\> cinst Kekiri.Tools`


## Why Kekiri?
Unlike other BDD frameworks that impose process overhead (management of feature files & shared steps, etc)
Kekiri allows developers to write BDD tests just as quickly and easily as they would technical tests.

The resulting tests are concise and highly portable.

## Example
For this **Test**, we will be implementing a basic calculator.

### Start with the test
Just like in TDD, when employing BDD, we write the test first:

```c#
    [Scenario]
    class Adding_two_numbers : Test {
        [Given]
        public void Given_a_calculator() {}

        [Given]
        public void The_user_enters_50() {}

        [Given]
        public void Next_the_user_enters_70() {}

        [When]
        public void When_the_user_presses_add() { throw new NotImplementedException(); }

        [Then]
        public void The_screen_should_display_result_of_120() {}
    }
```

If we were to run this test (even though it fails) we get a nice Cucumber-style feature output:

        Scenario: Adding two numbers
        Given a calculator
            And the user enters 50
            And next the user enters 70
        When the user presses add
        Then the screen should display result of 120

### Add the implementation
```c#
    [Scenario]
    class Adding_two_numbers : Test {
        private Calculator _calculator;

        [Given]
        public void Given_a_calculator() {
            _calculator = new Calculator();
        }

        [Given]
        public void The_user_enters_50() {
            _calculator.Operand1 = 50;
        }

        [Given]
        public void Next_the_user_enters_70() {
            _calculator.Operand2 = 70;
        }

        [When]
        public void When_the_user_presses_add() {
            _calculator.Add();
        }

        [Then]
        public void The_screen_should_display_result_of_120() {
            Assert.AreEqual(120, _calculator.Result);
        }
    }

    class Calculator {
        public decimal Operand1 { get; set; }
        public decimal Operand2 { get; set; }

        public decimal Result { get; set; }

        public void Add() {
            Result = Operand1 + Operand2;
        }
    }
```

### You're done!
Wasn't that painless?

---

## Supported Naming Conventions
Kekiri supports both Pascal case conventions (e.g. `WhenDoingTheThing`) just as it does
underscore convention (e.g. `When_doing_the_thing`).

---

## Other common use cases

### Expected Exceptions
```c#
    class Divide_by_zero : Test {
        readonly Calculator _calculator = new Calculator();

        [When, Throws]
        public void When_dividing() {
            _calculator.Divide();
        }

        [Then]
        public void It_should_throw_an_exception() {
            Catch<DivideByZeroException>();
        }
    }
```

Notice, here we've used the `[Throws]` attribute to inform the **Test** that throwing an
exception is the expected behavior.  In 1 or more `[Then]`s, the thrown type of exception must
be caught (using the templated method `Catch<>`).

   Scenario: Divide by zero
        When dividing
        Then it should throw an exception

### Data-driven
```c#
    [Scenario(Feature.Eating)]
    [Example(12, 5, 7)]
    [Example(20, 5, 15)]
    public class Eating_cucumbers : Test {
        private readonly int _start;
        private readonly int _eat;
        private readonly int _left;
        private int _cucumbers;

        public Eating_cucumbers(int start, int eat, int left) {
            _start = start;
            _eat = eat;
            _left = left;
        }

        [Given]
        public void Given_there_are_START_cucumbers() {
            _cucumbers = _start;
        }

        [When]
        public void When_I_eat_EAT_cucumbers() {
            _cucumbers -= _eat;
        }

        [Then]
        public void I_should_have_LEFT_cucumbers() {
            Assert.AreEqual(_left, _cucumbers);
        }
    }
```

        Scenario Outline: eating
        Given there are 12 cucumbers
        When i eat 5 cucumbers
        Then i should have 7 cucumbers

### IoC Tests
`PM> Install-Package Kekiri.IoC.Autofac`

#### Example
Consider this collection of classes:
```c#
        class Orchestrator {
            public Validator Validator { get; private set; }
            public Executor Executor { get; private set; }
            public IDataComponent DataComponent { get; private set; }

            public Orchestrator(
               Validator validator, Executor executor, IDataComponent dataComponent) {
                Validator = validator;
                Executor = executor;
                DataComponent = dataComponent;
            }

            public int Process() {
                var data = DataComponent.GetData();

                Validator.Validate(data);
                return Executor.Execute(data);
            }
        }

        class Validator {
            public void Validate(string input) {
                if (string.IsNullOrEmpty(input)) {
                    throw new ArgumentException("Must have a value", "input");
                }
            }
        }

        class Executor {
            public WordCounter WordCounter { get; set; }

            public Executor(WordCounter wordCounter) {
                WordCounter = wordCounter;
            }

            public int Execute(string input) {
                return WordCounter.CountWords(input);
            }
        }

        class WordCounter {
            public int CountWords(string sentence) {
                return sentence.Split(' ').Length;
            }
        }

        interface IDataComponent {
            string GetData();
        }

        class RealDataComponent : IDataComponent
        {
            public string GetData() {
                using (var connection = new SqlConnection("[YourDatabase]")) {
                    connection.Open();
                    using (var command = connection.CreateCommand()) {
                        command.CommandText = "SELECT YourField from [YourTable]";
                        return (string)command.ExecuteScalar();
                    }
                }
            }
        }

        class FakeDataComponent : IDataComponent
        {
            public string GetData() {
                return "all your base are belong to us";
            }
        }
```
Here's our test fixture:
```c#
    [Scenario]
    class Using_fakes_with_autofac : AutofacTest
    {
        private Orchestrator _orchestrator;

        [Given]
        public void Given() {
            Container.Register(new FakeDataComponent());
        }

        [When]
        public void When_resolving_an_instance() {
            _orchestrator = Container.Resolve<Orchestrator>();
        }

        [Then]
        public void It_uses_reals_for_everything() {
            _orchestrator.Validator.Should().BeOfType<Validator>();
            _orchestrator.Executor.Should().BeOfType<Executor>();
            _orchestrator.Executor.WordCounter.Should().BeOfType<WordCounter>();
        }

        [Then]
        public void But_explicitly_faked_objects() {
            _orchestrator.DataComponent.Should().BeOfType<FakeDataComponent>();
        }

        [Then]
        public void And_it_computes_the_right_result() {
            _orchestrator.Process().Should().Be(7);
        }
    }
```

When calling **Resolve**, a full object graph is created for **_orchestrator**.  Unless explicitly injected into the IoC container (**Container**), real objects are used!

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
