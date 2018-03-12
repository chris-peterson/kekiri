using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kekiri.Impl.Reporting
{
    class CompositeReportTarget : IReportTarget
    {
        readonly List<IReportTarget> _targets;

        CompositeReportTarget(IEnumerable<IReportTarget> targets)
        {
            _targets = new List<IReportTarget>(targets);
        }

        public static IReportTarget GetInstance()
        {
            var targets = new List<IReportTarget>();
            targets.Add(TraceReportTarget.GetInstance());

            var outputConfig = Environment.GetEnvironmentVariable("KEKIRI_OUTPUT");
            if (!string.IsNullOrWhiteSpace(outputConfig))
            {
                var split = outputConfig.Split(',')
                    .Select(s => s.Trim().ToLower())
                    .Distinct();
                if (split.Contains("console"))
                {
                    Trace.Listeners.Add(new TextWriterTraceListener(System.Console.Out));
                }
                if (split.Contains("files"))
                {
                    targets.Add(FeatureFileReportTarget.GetInstance());
                }
            }
            return new CompositeReportTarget(targets);
        }

        public void Report(ScenarioReportingContext scenario)
        {
            foreach (var target in _targets)
            {
                target.Report(scenario);
            }
        }
    }
}