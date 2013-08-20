# Kekiri
A .NET framework that supports writing low-ceremony BDD tests using Gherkin language.

Kekiri honors the conventions of the [cucumber language] (https://github.com/cucumber/cucumber/wiki/Feature-Introduction)

## Setup
`PM> Install-Package Kekiri`  
`C:\> cinst Kekiri.Tools`


## Why Kekiri?
Unlike other BDD frameworks that impose process overhead (management of feature files & shared steps, etc) 
Kekiri allows developers to write BDD tests just as quickly and easily as they would technical tests.

## Example
For this **ScenarioTest**, we will be implementing a basic calculator. 

### Start with the test
Just like in TDD, when employing BDD, we write the test first:

<pre lang="c#"><code>
    [Scenario]
    class Adding_two_numbers : ScenarioTest
    {
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
</code></pre>

When running the failing test, we get a scenario report:

        Scenario: Adding two numbers  
        Given a calculator  
            And the user enters 50  
            And next the user enters 70  
        When the user presses add  
        Then the screen should display result of 120

### Next, we add the implementation
<pre lang="c#"><code>
    [Scenario]
    class Adding_two_numbers : ScenarioTest
    {
        private Calculator _calculator;

        [Given]
        public void Given_a_calculator()
        {
            _calculator = new Calculator();
        }

        [Given]
        public void The_user_enters_50()
        {
            _calculator.Operand1 = 50;
        }

        [Given]
        public void Next_the_user_enters_70()
        {
            _calculator.Operand2 = 70;
        }

        [When]
        public void When_the_user_presses_add()
        {
            _calculator.Add();
        }

        [Then]
        public void The_screen_should_display_result_of_120()
        {
            Assert.AreEqual(120, _calculator.Result);
        }
    }
    
    public class Calculator
    {
        public decimal Operand1 { get; set; }
        public decimal Operand2 { get; set; }

        public decimal Result { get; set; }

        public void Add()
        {
            Result = Operand1 + Operand2;
        }
    }
</code></pre>

### That's it!
Now, let's add a test for an expected exception.

<pre lang="c#"><code>
    class When_dividing_by_zero : ScenarioTest
    {
        readonly Calculator _calculator = new Calculator();

        [When, Throws]
        public void When()
        {
            _calculator.Divide();
        }

        [Then]
        public void It_should_throw_an_exception()
        {
            Catch<DivideByZeroException>();
        }
    }
</code></pre>

Notice, here we've used the `[Throws]` attribute to inform the **ScenarioTest** that throwing an 
exception is the expected behavior.  In 1 or more `[Then]`s, the thrown type of exception must 
be caught (using the templated method `Catch<>`).  

Also, notice that this test is more terse than the previous example.  This is a stylistic choice.
By not using the `[Scenario]` attribute, and naming the class after the **When**, the test 
output is also more terse:

        When dividing by zero  
        Then it should throw an exception

### Supported Naming Conventions
Kekiri supports both Pascal case conventions (e.g. `WhenDoingTheThing`) just as it does 
underscore convention (e.g. `When_doing_the_thing`).

### Configuration Options
If you want to generate a .feature file as you run your unit tests, add to your test project's **App.config**, e.g.

```xml
  <configuration>  
    <system.diagnostics>  
      <trace autoflush="true" indentsize="4">  
        <listeners>  
          <add name="fileListener" type="System.Diagnostics.TextWriterTraceListener"  
             initializeData="YOUR_FEATURE_NAME.feature" />  
        </listeners>  
      </trace>  
    </system.diagnostics>  
  </configuration>  
```

Replacing **YOUR_FEATURE_NAME** with your feature's name.

## Contributing

1. Fork it
2. Create your feature branch `git checkout -b my-new-feature`
3. Commit your changes `git commit -am 'Added some feature'`
4. Push to the branch `git push origin my-new-feature`
5. Create new Pull Request
