using FluentAssertions;
using Kekiri.TestSupport.Scenarios.Reporting;

namespace Kekiri.UnitTests.Reporting
{
    [Scenario("Default step names")]
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
            ScenarioReport.Should().Be(string.Format(
                "{0} generating report for default step names scenario", Settings.GetStep(StepType.When)));
        }
    }
}