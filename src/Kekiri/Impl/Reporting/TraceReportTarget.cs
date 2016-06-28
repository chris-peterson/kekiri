using System;
using System.Diagnostics;

namespace Kekiri.Impl.Reporting
{
    class TraceReportTarget : IReportTarget
    {
        static readonly Lazy<TraceReportTarget> _target = new Lazy<TraceReportTarget>(() => new TraceReportTarget());

        TraceReportTarget()
        {
        }

        public static IReportTarget GetInstance()
        {
            return _target.Value;
        }

        public void Report(ScenarioReportingContext scenario)
        {
            Debug.WriteLine(scenario.CreateReport());
        }
    }
}