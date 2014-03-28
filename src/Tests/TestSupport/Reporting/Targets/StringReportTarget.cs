using Kekiri.Reporting;

namespace Kekiri.TestSupport.Reporting.Targets
{
    public class StringReportTarget : IReportTarget
    {
        public StringReportTarget()
        {
            ReportString = string.Empty;
        }

        public string ReportString { get; private set; }

        public void Report(ReportType reportType, ScenarioReportingContext scenario)
        {
            ReportString += scenario.CreateReportWithStandardSpacing(0);
        }
    }
}