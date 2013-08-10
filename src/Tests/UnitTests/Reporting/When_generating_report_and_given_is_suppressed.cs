using FluentAssertions;
using Kekiri.TestSupport.Scenarios.Reporting;

namespace Kekiri.UnitTests.Reporting
{
    [Scenario("The 'given' is suppressed")]
    class When_generating_report_and_given_is_suppressed : ReportingScenarioTest
    {
        [Given]
        public void Given()
        {
            Scenario = new When_generating_report_with_suppressed_given_scenario();
        }

        [Then]
        public void The_report_should_have_output_for_then()
        {
            ScenarioReport.Should().Contain(Settings.GetStep(StepType.Then));
        }

        [Then]
        public void And_when()
        {
            ScenarioReport.Should().Contain(Settings.GetStep(StepType.When));
        }

        [Then]
        public void But_not_for_given()
        {
            ScenarioReport.Should().NotContain(Settings.GetStep(StepType.Given));
        }
   }
}