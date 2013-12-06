using FluentAssertions;
using Kekiri.Config;
using Kekiri.TestSupport.Scenarios.Examples;

namespace Kekiri.UnitTests.Reporting
{
    public class When_generating_report_for_an_outlined_scenario : ReportingScenarioTest
    {
        [Given]
        public void Given()
        {
            Scenario = new Eating_cucumbers(12, 5, 7);
        }

        [Then]
        public void It_should_generate_the_correct_report()
        {
            ScenarioReport.Should().Be(
                string.Format(
                    "Scenario Outline: eating{0}{1}Given there are 12 cucumbers{0}{1}When i eat 5 cucumbers{0}{1}Then i should have 7 cucumbers",
                    Settings.GetSeperator(SeperatorType.Line),
                    Settings.GetSeperator(SeperatorType.Indent)));
        }
    }
}