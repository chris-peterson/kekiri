namespace Kekiri.Impl.Reporting
{
    interface IReportTarget
    {
        void Report(ScenarioReportingContext scenario);
    }
}