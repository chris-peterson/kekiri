using FluentAssertions;
using Kekiri.Config;
using Kekiri.TestSupport.Scenarios.Reporting;

namespace Kekiri.UnitTests.Reporting
{
    [Scenario("Custom steps")]
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
                string.Format(
                    "{0} one thing{3}{4}{5} another thing{3}{4}{5} yet another thing{3}{1} i open my eyes{3}{2} i see something{3}{4}{6} i dont see something else",
                    Settings.GetStep(StepType.Given),
                    Settings.GetStep(StepType.When),
                    Settings.GetStep(StepType.Then),
                    Settings.GetSeperator(SeperatorType.Line),
                    Settings.GetSeperator(SeperatorType.Indent),
                    Settings.GetToken(TokenType.And),
                    Settings.GetToken(TokenType.But)));
        }
    }
}