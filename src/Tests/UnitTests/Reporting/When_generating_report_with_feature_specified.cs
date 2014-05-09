using FluentAssertions;
using Kekiri.TestSupport.Scenarios.Reporting;

namespace Kekiri.UnitTests.Reporting
{
    class When_generating_report_with_feature_specified : ReportingScenarioTest
    {
        [Given]
        public void Given()
        {
            Scenario = new Feature_specified();
        }

        [Then]
        public void It_should_generate_the_proper_report()
        {
            ScenarioReport.Should().Be("Feature: Reporting\r\n\r\nScenario: Feature specified\r\n  Given precondition\r\n  When doing the deed\r\n  Then it should do the right thing");
        }
    }
}