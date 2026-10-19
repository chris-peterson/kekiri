using Kekiri.Internal.Reporting;
using Kekiri.Internal;

namespace Kekiri;

/// <summary>
/// The base a scenario fixture derives from. Its report target keeps the Gherkin lines in memory
/// so the runner can put them on the test node as well as on the console.
/// </summary>
public abstract class TestFixture : ScenarioBase, IRecordsSteps
{
    readonly CapturingReportTarget _captured = new CapturingReportTarget();

    CapturingReportTarget IRecordsSteps.Captured => _captured;

    internal override IReportTarget CreateReportTarget() => CompositeReportTarget.Including(_captured);
}
