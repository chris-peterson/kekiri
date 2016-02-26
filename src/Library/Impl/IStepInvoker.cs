namespace Kekiri.Impl
{
    interface IStepInvoker
    {
        void Invoke(object test);

        bool ExceptionExpected { get; set; }

        StepName Name { get; }

        StepType Type { get; }

        string SourceDescription { get; }
    }
}