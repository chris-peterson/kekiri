using AwesomeAssertions;
using Behavior;

namespace Behavior.Examples.ServiceProvider.Injection;

/// <summary>
/// A typed context the container builds, so its constructor names what the scenario needs.
/// </summary>
public class Resolving_a_service_the_app_registered : Scenarios<AppContext>
{
    [Scenario]
    public void The_test_double_replaces_the_apps_own_registration()
    {
        Given(the_app_is_running);
        When(resolving_foo);
        Then(the_test_double_is_what_arrives);
    }

    void the_app_is_running()
    {
    }

    void resolving_foo() => Context.Resolve();

    void the_test_double_is_what_arrives() => Context.Foo.Should().BeOfType<TestFoo>();
}
