using System.Collections.Generic;

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
            return new CompositeReportTarget(
                new[]
                {
                    TraceReportTarget.GetInstance(),
                    FeatureFileReportTarget.GetInstance()
                });
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