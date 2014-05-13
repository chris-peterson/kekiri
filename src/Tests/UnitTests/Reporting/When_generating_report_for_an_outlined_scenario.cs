using FluentAssertions;
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
                "Feature: TestSupport\r\n\r\nScenario Outline: Eating cucumbers\r\n  Given there are 12 cucumbers\r\n  When i eat 5 cucumbers\r\n  Then i should have 7 cucumbers");
        }
    }
}