using FluentAssertions;
using Kekiri.TestSupport.Scenarios.Reporting;

namespace Kekiri.UnitTests.Reporting
{
    [Scenario("The 'when' is suppressed")]
    class When_generating_report_and_when_is_suppressed : ReportingScenarioTest
    {
        [Given]
        public void Given()
        {
            Scenario = new When_generating_report_and_when_is_suppressed_scenario();
        }

        [Then]
        public void The_report_should_have_output_for_given()
        {
            ScenarioReport.Should().Contain(Settings.GetStep(StepType.Given));
        }

        [Then]
        public void And_then()
        {
            ScenarioReport.Should().Contain(Settings.GetStep(StepType.Then));
        }

        [Then]
        public void But_not_for_when()
        {
            ScenarioReport.Should().NotContain("When method is suppressed");
        }
    }
}