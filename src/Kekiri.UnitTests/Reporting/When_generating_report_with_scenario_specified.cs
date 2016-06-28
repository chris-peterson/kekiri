using FluentAssertions;
using Kekiri.TestSupport.Scenarios.Reporting;

namespace Kekiri.UnitTests.Reporting
{
    class When_generating_report_with_scenario_specified : ReportingScenarioTest
    {
        [Given]
        public void Given()
        {
            Scenario = new When_generating_report_with_scenario_specified_scenario();
        }

        [Then]
        public void It_should_generate_the_proper_report()
        {
            ScenarioReport.Should().Be(
                "Feature: TestSupport\r\n\r\nScenario: Test scenario\r\n  Given precondition\r\n  When doing the deed\r\n  Then it should do the right thing");
        }
    }
}