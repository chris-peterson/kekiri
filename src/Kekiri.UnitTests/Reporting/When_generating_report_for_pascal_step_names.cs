using FluentAssertions;
using Kekiri.TestSupport.Scenarios.Reporting;

namespace Kekiri.UnitTests.Reporting
{
    class When_generating_report_for_pascal_step_names : ReportingScenarioTest
    {
        [Given]
        public void Given()
        {
            Scenario = new PascalTest();
        }

        [Then]
        public void It_should_generate_the_correct_report()
        {
            ScenarioReport.Should().Be(
                "Feature: TestSupport\r\n\r\nScenario: Pascal test\r\n  Given an assumption\r\n  When performing the deed\r\n  Then it should do the right thing");
        }
    }
}