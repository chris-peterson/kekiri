using AwesomeAssertions;
using Behavior;

namespace Behavior.Examples.Resolution;

/// <summary>
/// No context type, so Context is a dynamic bag. The container comes from
/// <see cref="_BeforeTestRun"/>, and a registered fake wins over the auto-registered type.
/// </summary>
public class Resolving_a_service_with_a_fake : Scenarios
{
    [Scenario]
    public void A_registered_fake_is_what_gets_injected()
    {
        Given(a_fake_repository);
        When(the_service_does_its_work);
        Then(it_used_the_fake);
    }

    void a_fake_repository() => Container.Register(new FakeRepository());

    void the_service_does_its_work() =>
        Context.Result = Container.Resolve<Service>().DoWork();

    void it_used_the_fake() => ((string)Context.Result).Should().Be("data");
}
