namespace Kekiri.Impl
{
    interface IStepInvoker
    {
        void Invoke(ScenarioBase scenario);

        bool ExceptionExpected { get; set; }

        StepName Name { get; }

        StepType Type { get; }

        string SourceDescription { get; }
    }
}