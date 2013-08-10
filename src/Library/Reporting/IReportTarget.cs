using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kekiri.Reporting
{
    public interface IReportTarget
    {
        void Report(ReportType reportType, IScenarioReportingContext reportingContext);
    }

    public class TraceReportTarget : IReportTarget
    {
        private static readonly Lazy<TraceReportTarget> _target = new Lazy<TraceReportTarget>(() => new TraceReportTarget());

        public static TraceReportTarget GetInstance()
        {
            return _target.Value;
        }

        private TraceReportTarget()
        {
        }

        private string _previousFeatureKey;

        public void Report(ReportType reportType, IScenarioReportingContext reportingContext)
        {
            // only output current test info if running in R# -- it's not useful in command line builds
            if (reportType == ReportType.CurrentTest)
            {
                var executingProcessName = Process.GetCurrentProcess().ProcessName;

                if (executingProcessName.IndexOf("JetBrains.ReSharper", StringComparison.InvariantCultureIgnoreCase) == -1)
                {
                    return;
                }
            }

            int indentationLevel = 0;
            // avoid repeating the same feature over and over
            var featureKey = GetKey(reportingContext.FeatureReport);
            if (featureKey != null && featureKey == _previousFeatureKey)
            {
                indentationLevel = 1;
                reportingContext.FeatureReport.Clear();
            }

            Trace.WriteLine(reportingContext.CreateReportWithStandardSpacing(indentationLevel));

            _previousFeatureKey = featureKey;
        }

        private string GetKey(IList<string> report)
        {
            if (report == null || !report.Any())
            {
                return null;
            }

            return string.Join("-", report);
        }
    }
}