using System.Threading.Tasks;
using Behavior;

namespace Behavior.Examples.Orchestration;

public class Resolving_an_orchestrator : Step<Orchestration>
{
    public override Task ExecuteAsync()
    {
        Context.Orchestrator = Container.Resolve<Orchestrator>();

        return Task.CompletedTask;
    }
}
