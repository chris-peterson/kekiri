using System.Threading.Tasks;
using AwesomeAssertions;
using Behavior;

namespace Behavior.Examples.Orchestration;

public class Everything_else_is_real : Step<Orchestration>
{
    public override Task ExecuteAsync()
    {
        Context.Orchestrator.Validator.Should().BeOfType<Validator>();
        Context.Orchestrator.Executor.Should().BeOfType<Executor>();
        Context.Orchestrator.Executor.WordCounter.Should().BeOfType<WordCounter>();

        return Task.CompletedTask;
    }
}
