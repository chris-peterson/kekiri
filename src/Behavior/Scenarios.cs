using Behavior.Internal.Reporting;
using Behavior.Internal;

namespace Behavior;

/// <summary>
/// Same surface consumers already write against, minus the runner-specific base. The report
/// target is swapped for one that keeps the Gherkin lines in memory so the runner can put them
/// on the test node instead of only on the console.
/// </summary>
public abstract class Scenarios : ScenarioBase, IRecordsSteps
{
    readonly CapturingReportTarget _captured = new CapturingReportTarget();

    CapturingReportTarget IRecordsSteps.Captured => _captured;

    internal override IReportTarget CreateReportTarget() => _captured;
}
