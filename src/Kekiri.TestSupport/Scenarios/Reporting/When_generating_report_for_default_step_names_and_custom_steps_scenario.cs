namespace Kekiri.TestSupport.Scenarios.Reporting
{
    // Example from https://github.com/cucumber/cucumber/wiki/Given-When-Then
    public class When_generating_report_for_default_step_names_and_custom_steps_scenario : ReportingScenarioMetaTest
    {
        [Given]
        public void Given()
        {
        }

        [Given]
        public void Given_one_thing()
        {
        }

        [Given]
        public void And_another_thing()
        {
        }

        [Given]
        public void Yet_another_thing()
        {
        }

        [When]
        public void When_I_open_my_eyes()
        {
        }

        [Then]
        public void Then()
        {
        }

        [Then]
        public void Then_i_see_something()
        {
        }

        [Then]
        public void But_i_dont_see_something_else()
        {
        }
   }
}