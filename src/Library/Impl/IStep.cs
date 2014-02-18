namespace Kekiri.Impl
{
    internal interface IStep
    {
        void Invoke(ScenarioTest test);

        bool SuppressOutput { get; }

        bool ExceptionExpected { get; }

        string Name { get; }

        StepType Type { get; }

        string SourceDescription { get; }
    }
}