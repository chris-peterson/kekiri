using System.Threading.Tasks;
using Behavior;

namespace Behavior.Examples.Orchestration;

public class Fake_data_access : Step<Orchestration>
{
    public override Task ExecuteAsync()
    {
        Container.Register(new FakeDataComponent());

        return Task.CompletedTask;
    }
}
