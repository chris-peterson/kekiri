using Behavior.Internal.Reporting;
using Behavior.Internal;

namespace Behavior;

public abstract class Scenarios<TContext> : ScenarioBase<TContext>, IRecordsSteps
{
    readonly CapturingReportTarget _captured = new CapturingReportTarget();

    CapturingReportTarget IRecordsSteps.Captured => _captured;

    internal override IReportTarget CreateReportTarget() => _captured;
}
