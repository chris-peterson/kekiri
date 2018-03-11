using System.Threading.Tasks;

namespace Kekiri.Impl
{
    interface IStepInvoker
    {
        Task InvokeAsync(ScenarioBase scenario);

        bool ExceptionExpected { get; set; }

        StepName Name { get; }

        StepType Type { get; }

        string SourceDescription { get; }
    }
}