using System.Collections.Generic;
using System.Linq;
using Kekiri.Impl.Reporting;

namespace Kekiri.Mtp.Internal
{
    /// <summary>
    /// Kekiri already models a scenario's Gherkin text through IReportTarget, for console and
    /// .feature output. Capturing it instead is what lets the runner put the steps on the test node.
    /// </summary>
    class CapturingReportTarget : IReportTarget
    {
        public string FeatureName { get; private set; }

        public IReadOnlyList<string> Steps { get; private set; } = new string[0];

        public void Report(ScenarioReportingContext scenario)
        {
            FeatureName = scenario.FeatureName;
            Steps = scenario.StepReport.ToList();
        }
    }
}
