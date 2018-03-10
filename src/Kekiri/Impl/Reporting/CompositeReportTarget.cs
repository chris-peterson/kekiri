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
                    // cpeterson TODO: file-based target is not thread-safe, so it fails with xUnit's parallel test execution
                    //FeatureFileReportTarget.GetInstance()
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