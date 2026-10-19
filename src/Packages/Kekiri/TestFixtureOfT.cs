using Kekiri.Internal.Reporting;
using Kekiri.Internal;

namespace Kekiri;

public abstract class TestFixture<TContext> : ScenarioBase<TContext>, IRecordsSteps
{
    readonly CapturingReportTarget _captured = new CapturingReportTarget();

    CapturingReportTarget IRecordsSteps.Captured => _captured;

    internal override IReportTarget CreateReportTarget() => CompositeReportTarget.Including(_captured);
}
