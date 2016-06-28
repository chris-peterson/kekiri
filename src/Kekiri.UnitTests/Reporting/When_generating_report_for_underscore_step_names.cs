using FluentAssertions;
using Kekiri.TestSupport.Scenarios.Reporting;

namespace Kekiri.UnitTests.Reporting
{
    class When_generating_report_for_underscore_step_names : ReportingScenarioTest
    {
        [Given]
        public void Given()
        {
            Scenario = new When_generating_report_for_underscore_step_names_scenario();
        }

        [Then]
        public void It_should_generate_the_correct_report()
        {
            ScenarioReport.Should().Be(
                "Feature: TestSupport\r\n\r\nScenario: When generating report for underscore step names scenario\r\n  Given an assumption\r\n  When performing the deed\r\n  Then it should do the right thing");
        }
    }
}