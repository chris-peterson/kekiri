using FluentAssertions;
using Kekiri.Config;
using Kekiri.TestSupport.Scenarios.Reporting;

namespace Kekiri.UnitTests.Reporting
{
    [Scenario("Pascal naming")]
    class When_generating_report_for_pascal_step_names : ReportingScenarioTest
    {
        [Given]
        public void Given()
        {
            Scenario = new When_generating_report_for_pascal_step_names_scenario();
        }

        [Then]
        public void It_should_generate_the_correct_report()
        {
            ScenarioReport.Should().Be(
                string.Format("{0} an assumption{3}{1} performing the deed{3}{2} it should do the right thing",
                              Settings.GetStep(StepType.Given),
                              Settings.GetStep(StepType.When),
                              Settings.GetStep(StepType.Then),
                              Settings.GetSeperator(SeperatorType.Line)));
        }
    }
}