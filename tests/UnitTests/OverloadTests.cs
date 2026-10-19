using System;
using System.Threading.Tasks;
using Kekiri.Tests.Fixtures;

namespace Kekiri.Tests;

public class OverloadTests
{
    [Test]
    public async Task Every_given_and_then_overload_runs_in_order_with_its_arguments()
    {
        var run = await Run.Scenario<Overloads>(f => f.Every_given_and_then());

        await Assert.That(run.Failure).IsNull();
        await Assert.That(string.Join(" | ", run.Fixture.Calls)).IsEqualTo(string.Join(" | ",
            "zero", "zero", "zero async", "one 1", "one async 1", "two 1 2", "two async 1 2",
            "three 1 2 3", "three async 1 2 3", "step and",
            "zero async", "one 1", "one async 1", "two 1 2", "two async 1 2", "three 1 2 3",
            "three async 1 2 3", "step given",
            "zero",
            "zero", "zero", "zero async", "one 1", "one async 1", "two 1 2", "two async 1 2",
            "three 1 2 3", "three async 1 2 3", "step but",
            "zero async", "one 1", "one async 1", "two 1 2", "two async 1 2", "three 1 2 3",
            "three async 1 2 3", "step then"));
    }

    [Test]
    [Arguments(nameof(Overloads.When_async), "zero async")]
    [Arguments(nameof(Overloads.When_with_one), "one 1")]
    [Arguments(nameof(Overloads.When_async_with_one), "one async 1")]
    [Arguments(nameof(Overloads.When_with_two), "two 1 2")]
    [Arguments(nameof(Overloads.When_async_with_two), "two async 1 2")]
    [Arguments(nameof(Overloads.When_with_three), "three 1 2 3")]
    [Arguments(nameof(Overloads.When_async_with_three), "three async 1 2 3")]
    [Arguments(nameof(Overloads.When_a_step_class), "step when")]
    public async Task Every_when_overload_runs_with_its_arguments(string scenario, string call)
    {
        var run = await Run.Scenario<Overloads>(f =>
            typeof(Overloads).GetMethod(scenario)!.Invoke(f, Array.Empty<object>()));

        await Assert.That(run.Failure).IsNull();
        await Assert.That(string.Join(" | ", run.Fixture.Calls)).IsEqualTo(call);
    }

    [Test]
    public async Task Arguments_the_step_name_has_no_placeholder_for_are_appended()
    {
        var run = await Run.Scenario<Overloads>(f => f.When_with_two());

        await Assert.That(run.Steps[1]).IsEqualTo("When two 1 2");
    }

    [Test]
    public async Task A_step_class_is_named_after_its_type_and_arguments()
    {
        var run = await Run.Scenario<Overloads>(f => f.When_a_step_class());

        await Assert.That(run.Steps[1]).IsEqualTo("When recording step when");
    }
}
