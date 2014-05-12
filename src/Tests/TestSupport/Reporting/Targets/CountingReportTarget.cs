using Kekiri.Reporting;

namespace Kekiri.TestSupport.Reporting.Targets
{
    internal class CountingReportTarget : IReportTarget
    {
        public int WriteCount { get; set; }

        public void Report(ScenarioReportingContext scenario)
        {
            WriteCount++;
        }
    }
}