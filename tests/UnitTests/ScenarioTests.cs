using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kekiri.Tests;

public class ScenarioTests
{
    [Test]
    public async Task An_example_row_reads_as_a_reader_would_write_it()
    {
        var scenario = Discovered.All().Single(s => s.ScenarioName == "Every kind of value");

        await Assert.That(scenario.Title)
            .IsEqualTo("Every kind of value [null, \"\", true, false, [1, 2], 1.5]");
        await Assert.That((await scenario.RunAsync()).Failure).IsNull();
    }

    [Test]
    public async Task An_example_value_converts_to_the_parameter_type()
    {
        var outcome = await Discovered.Named("Converts to the parameter type [1, 2]").RunAsync();

        await Assert.That(outcome.Failure).IsNull();
    }

    [Test]
    public async Task A_value_that_cannot_convert_fails_the_scenario()
    {
        var outcome = await Discovered.Named("A value that cannot convert [System.Int32]").RunAsync();

        await Assert.That(outcome.Failure).IsTypeOf<ArgumentException>();
    }

    [Test]
    public async Task A_row_with_the_wrong_number_of_values_fails_the_scenario()
    {
        var outcome = await Discovered.Named("Too many values [1, 2]").RunAsync();

        await Assert.That(outcome.Failure!.Message)
            .IsEqualTo("Too_many_values takes 1 argument(s), but the [Example] supplies 2.");
    }

    [Test]
    public async Task A_scenario_records_its_source_location_for_navigation()
    {
        var location = Discovered.Named("Adding 1 and 2").FileLocation;

        await Assert.That(location!.FilePath.EndsWith("Adding_numbers.cs")).IsTrue();
        await Assert.That(location.LineSpan.Start.Line).IsGreaterThan(0);
    }

    [Test]
    public async Task A_scenario_without_a_source_location_has_none_to_offer()
    {
        await Assert.That(Discovered.Named("Has no source location").FileLocation).IsNull();
    }

    [Test]
    public async Task A_generic_scenario_reports_its_arity()
    {
        var identifier = Discovered.Named("Generic").MethodIdentifier;

        await Assert.That(identifier.MethodArity).IsEqualTo(1);
    }

    [Test]
    public async Task A_scenario_that_is_not_a_test_fixture_records_no_steps()
    {
        var outcome = await Discovered.Named("Has no source location").RunAsync();

        await Assert.That(outcome.Failure).IsNull();
        await Assert.That(outcome.Steps.Count).IsEqualTo(0);
    }

    [Test]
    public async Task A_fixture_that_cannot_be_created_fails_with_no_steps()
    {
        var outcome = await Discovered.Named("Needs a constructor argument").RunAsync();

        await Assert.That(outcome.Failure).IsTypeOf<MissingMethodException>();
        await Assert.That(outcome.Steps.Count).IsEqualTo(0);
    }

    [Test]
    public async Task A_fixture_outside_a_namespace_is_its_own_feature()
    {
        var scenario = Discovered.Named("Still has a feature");

        await Assert.That(scenario.FeatureName).IsEqualTo("Without a namespace");
        await Assert.That(scenario.NodePath).IsEqualTo("/Kekiri.UnitTests//Without_a_namespace/Still_has_a_feature");
    }

    [Test]
    public async Task A_fixture_outside_a_namespace_runs_and_reports()
    {
        var outcome = await Discovered.Named("Still has a feature").RunAsync();

        await Assert.That(outcome.Failure).IsNull();
        await Assert.That(outcome.Steps.Count).IsEqualTo(2);
    }
}
