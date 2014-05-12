namespace Kekiri.Impl
{
    internal interface IStepInvoker
    {
        void Invoke(object test);

        bool ExceptionExpected { get; }

        int Order { get; }

        StepName Name { get; }

        StepType Type { get; }

        string SourceDescription { get; }
    }
}