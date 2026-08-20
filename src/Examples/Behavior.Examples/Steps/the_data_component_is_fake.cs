using System.Threading.Tasks;
using AwesomeAssertions;
using Behavior;

namespace Behavior.Examples.Orchestration;

public class the_data_component_is_fake : Step<Orchestration>
{
    public override Task ExecuteAsync()
    {
        Context.Orchestrator.DataComponent.Should().BeOfType<FakeDataComponent>();

        return Task.CompletedTask;
    }
}
