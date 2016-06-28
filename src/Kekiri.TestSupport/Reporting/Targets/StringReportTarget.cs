using Kekiri.Impl.Reporting;

namespace Kekiri.TestSupport.Reporting.Targets
{
    internal class StringReportTarget : IReportTarget
    {
        public StringReportTarget()
        {
            ReportString = string.Empty;
        }

        public string ReportString { get; private set; }

        public void Report(ScenarioReportingContext scenario)
        {
            ReportString += scenario.CreateReport();
        }
    }
}