using FluentAssertions;
using Kekiri.TestSupport.Scenarios.Reporting;

namespace Kekiri.UnitTests.Reporting
{
    class When_generating_report_for_default_step_names_and_custom_steps : ReportingScenarioTest
    {
        [Given]
        public void Given()
        {
            Scenario = new When_generating_report_for_default_step_names_and_custom_steps_scenario();
        }

        [Then]
        public void It_should_generate_the_correct_report()
        {
            ScenarioReport.Should().Be(
                "Feature: TestSupport\r\n\r\nScenario: When generating report for default step names and custom steps scenario\r\n  Given one thing\r\n    And another thing\r\n    And yet another thing\r\n  When i open my eyes\r\n  Then i see something\r\n    But i dont see something else");
        }
    }
}