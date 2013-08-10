# Kekiri
A .NET framework that supports writing low-ceremony BDD tests using Gherkin language.

Kekiri honors the conventions of the [cucumber language] (https://github.com/cucumber/cucumber/wiki/Feature-Introduction)

## Setup
`PM> Install-Package Kekiri`

## Why Kekiri?
Unlike other BDD frameworks that impose process overhead (e.g. management of feature files, shared steps, etc) 
Kekiri allows developers to write BDD tests just as quickly and easily as they would technical tests.

## Example
For this `ScenarioTest`, we will be implementing a basic calculator. 

### Start with the test
Just like in TDD, in BDD, we typically write the test first:

<pre lang="c#"><code>
    [Scenario]
    class Adding_two_numbers : ScenarioTest
    {
        [Given]
        public void Given_a_calculator()
        {
        }

        [Given]
        public void The_user_enters_50()
        {
        }

        [Given]
        public void Next_the_user_enters_70()
        {
        }

        [When]
        public void When_the_user_presses_add()
        {
           throw new NotImplementedException();
        }

        [Then]
        public void The_screen_should_display_result_of_120()
        { 
        }
    }}
</code></pre>

When running this, we get a scenario report:

Scenario: Adding two numbers  
Given a calculator  
  And the user enters 50  
  And next the user enters 70  
When the user presses add  
Then the screen should display result of 120`  

## Contributing

1. Fork it
2. Create your feature branch `git checkout -b my-new-feature`
3. Commit your changes `git commit -am 'Added some feature'`
4. Push to the branch `git push origin my-new-feature`
5. Create new Pull Request
