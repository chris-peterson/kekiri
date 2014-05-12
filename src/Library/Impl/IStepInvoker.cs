namespace Kekiri.Impl
{
    internal interface IStepInvoker
    {
        void Invoke(object test);

        bool ExceptionExpected { get; }

        StepName Name { get; }

        StepType Type { get; }

        string SourceDescription { get; }
    }
}