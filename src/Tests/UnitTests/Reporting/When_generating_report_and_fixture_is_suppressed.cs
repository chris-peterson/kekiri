using FluentAssertions;
using Kekiri.TestSupport.Scenarios.Reporting;

namespace Kekiri.UnitTests.Reporting
{
    [Scenario("Fixture reporting suppressed")]
    class When_generating_report_and_fixture_is_suppressed : ReportingScenarioTest
    {
        [Given]
        public void Given()
        {
            Scenario = new When_generating_report_and_fixture_is_suppressed_scenario();
        }

        [Then]
        public void It_should_not_output_anything()
        {
            ScenarioReport.Should().BeEmpty();
        }
    }
}