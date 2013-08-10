using Kekiri.Reporting;

namespace Kekiri.TestSupport.Scenarios.Reporting
{
    public class When_generating_report_and_when_is_suppressed_scenario : ReportingScenarioMetaTest
    {
        [Given]
        public void Given_something()
        {
        }

        [When]
        [SuppressOutput]
        public void When()
        {
        }

        [Then]
        public void Then_something()
        {
        }
    }
}