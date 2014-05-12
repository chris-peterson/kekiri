using Kekiri.Reporting;

namespace Kekiri.TestSupport.Reporting.Targets
{
    internal class StringReportTarget : IReportTarget
    {
        private readonly bool _includeFeatureReport;

        public StringReportTarget(bool includeFeatureReport)
        {
            _includeFeatureReport = includeFeatureReport;
            ReportString = string.Empty;
        }

        public string ReportString { get; private set; }

        public void Report(ScenarioReportingContext scenario)
        {
            ReportString += scenario.CreateReport(_includeFeatureReport);
        }
    }
}