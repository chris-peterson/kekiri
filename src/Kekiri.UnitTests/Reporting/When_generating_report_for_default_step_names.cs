using FluentAssertions;
using Kekiri.TestSupport.Scenarios.Reporting;

namespace Kekiri.UnitTests.Reporting
{
    class When_generating_report_for_default_step_names : ReportingScenarioTest
    {
        [Given]
        public void Given()
        {
            Scenario = new When_generating_report_for_default_step_names_scenario();
        }

        [Then]
        public void It_should_generate_a_brief_report()
        {
            ScenarioReport.Should().Be(
                "Feature: TestSupport\r\n\r\nScenario: When generating report for default step names scenario\r\n  When generating report for default step names scenario");
        }
    }
}