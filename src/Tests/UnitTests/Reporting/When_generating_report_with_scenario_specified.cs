using FluentAssertions;
using Kekiri.Config;
using Kekiri.TestSupport.Scenarios.Reporting;

namespace Kekiri.UnitTests.Reporting
{
    [Scenario("Scenario specified")]
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
            ScenarioReport.Should().Be(string.Format(
                "{5}Scenario Description{3}{4}{0} precondition{3}{4}{1} doing the deed{3}{4}{2} it should do the right thing", 
                Settings.GetStep(StepType.Given),
                Settings.GetStep(StepType.When),
                Settings.GetStep(StepType.Then),
                Settings.GetSeperator(SeperatorType.Line),
                Settings.GetSeperator(SeperatorType.Indent),
                Settings.GetToken(TokenType.Scenario)));
        }
    }
}