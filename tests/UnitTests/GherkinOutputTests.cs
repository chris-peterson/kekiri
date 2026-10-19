using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kekiri.Mtp;
using Kekiri.Mtp.Internal;

namespace Kekiri.Tests;

public class GherkinOutputTests
{
    static async Task<Platform.Output> Print(params string[] titles)
    {
        var output = new Platform.Output();
        var formatter = new GherkinFormatter(output, new Platform.Producer());

        foreach (var title in titles)
        {
            var scenario = Discovered.Named(title);
            await formatter.ScenarioAsync(scenario, await scenario.RunAsync(), TimeSpan.FromMilliseconds(3), CancellationToken.None);
        }

        return output;
    }

    [Test]
    public async Task A_passing_scenario_prints_its_steps_and_a_green_verdict()
    {
        var output = await Print("Adding 1 and 2");

        await Assert.That(output.Text).IsEqualTo(string.Join("\n",
            "",
            "Feature: Arithmetic",
            "",
            "  Scenario: Adding 1 and 2",
            "    Given a running total",
            "    When adding 1 2",
            "    Then the total is 3",
            "    ✓ passed (3ms)"));
        await Assert.That(output.Lines[1].Color).IsEqualTo(ConsoleColor.Cyan);
        await Assert.That(output.Lines.Last().Color).IsEqualTo(ConsoleColor.Green);
    }

    [Test]
    public async Task The_feature_heading_is_printed_once_per_feature()
    {
        var output = await Print("Adding 1 and 2", "Carries its tags", "The last step throws");

        await Assert.That(output.Lines.Count(l => l.Text == "Feature: Arithmetic")).IsEqualTo(1);
        await Assert.That(output.Lines.Count(l => l.Text == "Feature: Broken")).IsEqualTo(1);
    }

    [Test]
    public async Task A_failing_step_is_marked_in_place_with_its_cause()
    {
        var output = await Print("The last step throws");

        var marked = output.Lines.SkipWhile(l => l.Text != "  ✗ Then a step that throws").ToArray();
        await Assert.That(marked[0].Color).IsEqualTo(ConsoleColor.Red);
        await Assert.That(marked[1].Text).IsEqualTo("      the cause");
    }

    [Test]
    public async Task A_failure_outside_any_step_gets_a_line_of_its_own()
    {
        var output = await Print("Too many values [1, 2]");

        await Assert.That(output.Lines[^2].Text).IsEqualTo("    (no steps recorded)");
        await Assert.That(output.Lines[^1].Text)
            .IsEqualTo("    ✗ Too_many_values takes 1 argument(s), but the [Example] supplies 2.");
        await Assert.That(output.Lines[^1].Color).IsEqualTo(ConsoleColor.Red);
    }
}
