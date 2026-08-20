using System.Threading.Tasks;
using AwesomeAssertions;
using Behavior;

namespace Behavior.Examples.Orchestration;

public class it_computes_the_right_result : Step<Orchestration>
{
    public override async Task ExecuteAsync()
    {
        await Task.Yield();

        Context.Orchestrator.Process().Should().Be(7);
    }
}
