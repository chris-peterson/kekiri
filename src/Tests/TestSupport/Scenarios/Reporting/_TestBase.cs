using Kekiri.Reporting;
using Kekiri.TestSupport.Reporting.Targets;

namespace Kekiri.TestSupport.Scenarios.Reporting
{
    public class ReportingScenarioMetaTest : ScenarioTest
    {
        private StringReportTarget _target;

        public string Report { get { return _target.ReportString; } }

        internal override IReportTarget CreateReportTarget()
        {
            if (_target == null)
            {
                _target = new StringReportTarget(IncludeFeatureReport);
            }

            return _target;
        }

        public bool IncludeFeatureReport { get; set; }
    }
}