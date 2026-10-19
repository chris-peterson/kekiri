using System.Threading.Tasks;

namespace Kekiri.Internal;

interface IStepInvoker
{
    Task InvokeAsync(ScenarioBase scenario);

    bool ExceptionExpected { get; set; }

    StepName Name { get; }

    StepType Type { get; }

    string SourceDescription { get; }
}
