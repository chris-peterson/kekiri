namespace Behavior.Internal.Reporting;

interface IReportTarget
{
    void Report(ScenarioReportingContext scenario);
}
