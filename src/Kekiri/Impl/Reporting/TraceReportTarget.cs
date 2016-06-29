using System;
using System.Diagnostics;

namespace Kekiri.Impl.Reporting
{
    class DebugReportTarget : IReportTarget
    {
        static readonly Lazy<DebugReportTarget> _target = new Lazy<DebugReportTarget>(() => new DebugReportTarget());

        DebugReportTarget()
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