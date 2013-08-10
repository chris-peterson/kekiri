using Kekiri.Reporting;

namespace Kekiri.TestSupport.Scenarios.Reporting
{
    public class When_generating_report_and_then_is_suppressed_scenario : ReportingScenarioMetaTest
    {
        [Given]
        public void Given_something()
        {
        }

        [When]
        public void When()
        {
        }

        [Then]
        [SuppressOutput]
        public void Then_something()
        {
        }
    }
}