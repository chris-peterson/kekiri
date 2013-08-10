using Kekiri.Reporting;

namespace Kekiri.TestSupport.Scenarios.Reporting
{
    public class When_generating_report_with_suppressed_given_scenario : ReportingScenarioMetaTest
    {
        [Given]
        [SuppressOutput]
        public void Given()
        {
        }

        [When]
        public void When()
        {
        }

        [Then]
        public void Then_something()
        {
        }
    }
}