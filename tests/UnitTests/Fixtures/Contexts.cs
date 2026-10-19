using System;
using System.Threading.Tasks;

namespace Kekiri.Tests.Fixtures;

public class Untyped_context : TestFixture
{
    public object Seen { get; private set; }

    public void Steps_share_a_dynamic_context()
    {
        Given(a_value);
        When<Reading_step>();
        Then(nothing);
    }

    void a_value() => Context.Value = 42;

    void nothing() => Seen = Context.Seen;
}

public class Reading_step : Step
{
    public override Task ExecuteAsync()
    {
        Context.Seen = Context.Value;
        return Task.CompletedTask;
    }
}

public class Total
{
    public int Value { get; set; }
}

public class Typed_context : TestFixture<Total>
{
    public void Steps_share_a_typed_context()
    {
        Given(a_value);
        When<Doubling_step>();
        Then(nothing);
    }

    void a_value() => Context.Value = 21;

    void nothing()
    {
    }

    public int Value => Context.Value;
}

public class Doubling_step : Step<Total>
{
    public override Task ExecuteAsync()
    {
        Context.Value *= 2;
        return Task.CompletedTask;
    }
}

/// <summary>No parameterless constructor, so the context has to come from the container.</summary>
public class Resolved_total
{
    public Resolved_total(string source)
    {
        Source = source;
    }

    public string Source { get; }
}

public class Resolved_context : TestFixture<Resolved_total>
{
    public string Source { get; private set; }

    public void The_context_comes_from_the_container()
    {
        When(reading);
        Then(nothing);
    }

    void reading() => Source = Context.Source;

    void nothing()
    {
    }
}

public class Container_from_a_step : TestFixture
{
    public void A_step_class_resolves_from_the_container()
    {
        When<Resolving_step>();
        Then(nothing);
    }

    void nothing()
    {
    }
}

public class Resolving_step : Step
{
    public override Task ExecuteAsync()
    {
        Context.Resolved = Container.Resolve<Total>();
        return Task.CompletedTask;
    }
}

public class Ambiguous_step : Step
{
    public Ambiguous_step()
    {
    }

    public Ambiguous_step(int value)
    {
    }

    public override Task ExecuteAsync() => Task.CompletedTask;
}

public class Hidden_step : Step
{
    Hidden_step()
    {
    }

    public override Task ExecuteAsync() => Task.CompletedTask;
}

public class Step_with_two_constructors : TestFixture
{
    public void Names_a_step_class_with_two_constructors()
    {
        When<Ambiguous_step>(1);
        Then(nothing);
    }

    public void Names_a_step_class_with_no_public_constructor()
    {
        When<Hidden_step>(new object[] { null });
        Then(nothing);
    }

    void nothing()
    {
    }
}

public class Plain_scenario : ScenarioBase
{
    public void Runs_without_a_test_fixture()
    {
        When(nothing);
        Then(nothing);
    }

    public Container ItsContainer => Container;

    void nothing()
    {
    }
}
