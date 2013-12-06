using Kekiri.Reporting;
using Kekiri.TestSupport.Reporting.Targets;

namespace Kekiri.TestSupport.Scenarios.Reporting
{
    public class ReportingScenarioMetaTest : ScenarioTest
    {
        private readonly StringReportTarget _target = new StringReportTarget();

        public string Report { get { return _target.ReportString; } }

        protected override IReportTarget CreateReportTarget()
        {
            return _target;
        }
    }
}