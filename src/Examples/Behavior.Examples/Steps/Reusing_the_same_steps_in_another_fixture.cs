using Behavior;

namespace Behavior.Examples.Orchestration;

public class Reusing_the_same_steps_in_another_fixture : Scenarios<Orchestration>
{
    [Scenario]
    public void The_same_steps_compose_into_a_shorter_scenario()
    {
        Given<Fake_data_access>();
        When<Resolving_an_orchestrator>();
        Then<it_computes_the_right_result>();
    }
}
