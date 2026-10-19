namespace Kekiri.Internal.Reporting;

interface IReportTarget
{
    void Report(ScenarioReportingContext scenario);
}
