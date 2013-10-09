using Kekiri.Reporting;

namespace Kekiri.TestSupport.Reporting.Targets
{
    public class CountingReportTarget : IReportTarget
    {
        public int WriteCount { get; set; }

        public void Report(ReportType reportType, ScenarioReportingContext reportingContext)
        {
            WriteCount++;
        }
    }
}