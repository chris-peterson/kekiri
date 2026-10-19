using System.Threading.Tasks;
using Kekiri.Internal.Exceptions;
using Kekiri.Tests.Fixtures;

namespace Kekiri.Tests;

public class ContextTests
{
    [Test]
    public async Task Steps_share_a_dynamic_context()
    {
        var run = await Run.Scenario<Untyped_context>(f => f.Steps_share_a_dynamic_context());

        await Assert.That(run.Failure).IsNull();
        await Assert.That(run.Fixture.Seen).IsEqualTo(42);
    }

    [Test]
    public async Task Steps_share_a_typed_context()
    {
        var run = await Run.Scenario<Typed_context>(f => f.Steps_share_a_typed_context());

        await Assert.That(run.Failure).IsNull();
        await Assert.That(run.Fixture.Value).IsEqualTo(42);
    }

    [Test]
    public async Task A_context_with_no_parameterless_constructor_comes_from_the_container()
    {
        using (ContainerSetup.Use(() => new Fixed_container(new Resolved_total("the container"))))
        {
            var run = await Run.Scenario<Resolved_context>(f => f.The_context_comes_from_the_container());

            await Assert.That(run.Failure).IsNull();
            await Assert.That(run.Fixture.Source).IsEqualTo("the container");
        }
    }

    [Test]
    public async Task A_step_class_resolves_from_the_scenario_container()
    {
        var run = await Run.Scenario<Container_from_a_step>(f => f.A_step_class_resolves_from_the_container());

        await Assert.That(run.Failure).IsNull();
        await Assert.That((object)run.Fixture.Context.Resolved).IsTypeOf<Total>();
    }

    [Test]
    public async Task A_step_class_needs_one_constructor()
    {
        var run = await Run.Scenario<Step_with_two_constructors>(f => f.Names_a_step_class_with_two_constructors());

        await Assert.That(run.Failure).IsTypeOf<ConstructorNotFound>();
        await Assert.That(run.Failure.Message.Contains("When Ambiguous_step (Int32 1), found 2")).IsTrue();
    }

    [Test]
    public async Task A_step_class_with_no_public_constructor_is_reported_the_same_way()
    {
        var run = await Run.Scenario<Step_with_two_constructors>(f => f.Names_a_step_class_with_no_public_constructor());

        await Assert.That(run.Failure).IsTypeOf<ConstructorNotFound>();
        await Assert.That(run.Failure.Message.Contains("Hidden_step (null ), found 0")).IsTrue();
    }

    [Test]
    public async Task A_scenario_that_is_not_a_test_fixture_still_runs()
    {
        var run = await Run.Scenario<Plain_scenario>(f => f.Runs_without_a_test_fixture());

        await Assert.That(run.Failure).IsNull();
        await Assert.That(run.Steps.Count).IsEqualTo(0);
    }

    [Test]
    public async Task A_scenario_resolves_one_container_for_its_lifetime()
    {
        var scenario = new Plain_scenario();

        await Assert.That(ReferenceEquals(scenario.ItsContainer, scenario.ItsContainer)).IsTrue();
    }

    [Test]
    public async Task Without_a_container_factory_the_failure_says_what_is_missing()
    {
        using (ContainerSetup.Use(null))
        {
            var scenario = new Plain_scenario();

            var failure = Assert.Throws<ContainerFactoryNotInitialized>(() => _ = scenario.ItsContainer);

            await Assert.That(failure.Message.Contains("Kekiri.IoC.Autofac")).IsTrue();
        }
    }

    sealed class Fixed_container : Container
    {
        readonly object _instance;

        public Fixed_container(object instance)
        {
            _instance = instance;
        }

        protected override T OnResolve<T>() => (T)_instance;
    }
}
