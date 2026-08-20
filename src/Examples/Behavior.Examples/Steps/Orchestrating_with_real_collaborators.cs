using Behavior;

namespace Behavior.Examples.Orchestration;

/// <summary>
/// Steps written as classes rather than methods, so two fixtures can share them. They read the
/// same typed context the fixture does, and they reach the container the same way.
/// </summary>
public class Orchestrating_with_real_collaborators : Scenarios<Orchestration>
{
    [Scenario]
    public void Only_the_data_access_is_faked()
    {
        Given<Fake_data_access>();
        When<Resolving_an_orchestrator>();
        Then<Everything_else_is_real>()
            .But<the_data_component_is_fake>()
            .And<it_computes_the_right_result>();
    }
}
