using Kekiri.Reporting;
using Kekiri.TestSupport.Reporting.Targets;

namespace Kekiri.TestSupport.Scenarios.Reporting
{
    [ScenarioBase(Feature.TestSupport)]
    public class ReportingScenarioMetaTest : Test
    {
        private StringReportTarget _target;

        public string Report { get { return _target.ReportString; } }

        internal override IReportTarget CreateReportTarget()
        {
            return _target ?? (_target = new StringReportTarget());
        }
    }
}