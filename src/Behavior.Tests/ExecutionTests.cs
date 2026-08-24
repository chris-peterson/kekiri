using System.Linq;
using System.Threading.Tasks;
using Behavior.Internal;

namespace Behavior.Tests;

public class ExecutionTests
{
    [Test]
    public async Task A_passing_scenario_records_its_gherkin()
    {
        var outcome = await Discovered.Named("Adding 1 and 2").RunAsync();

        await Assert.That(outcome.Failure).IsNull();
        await Assert.That(outcome.Steps.Count).IsEqualTo(3);
        await Assert.That(outcome.Steps.First().Trim()).IsEqualTo("Given a running total");
    }

    [Test]
    public async Task An_example_row_binds_its_arguments()
    {
        var outcome = await Discovered.Named("Adding any two numbers [2, 3, 5]").RunAsync();

        await Assert.That(outcome.Failure).IsNull();
    }

    [Test]
    public async Task A_typed_context_carries_state_between_steps()
    {
        var outcome = await Discovered
            .Named("The context carries state between steps")
            .RunAsync();

        await Assert.That(outcome.Failure).IsNull();
    }

    [Test]
    public async Task Arguments_with_no_example_row_fail_when_the_scenario_runs()
    {
        var outcome = await Discovered.Named("Takes an argument with no example []").RunAsync();

        await Assert.That(outcome.Failure).IsNotNull();
        await Assert.That(outcome.Failure.Message.Contains("takes 1 argument(s)")).IsTrue();
    }

    [Test]
    public async Task A_failing_step_reports_the_cause_rather_than_the_wrapper()
    {
        var outcome = await Discovered.Named("The last step throws").RunAsync();

        await Assert.That(outcome.Failure).IsNotNull();
        await Assert.That(FailureReport.OneLine(outcome.Failure)).IsEqualTo("the cause");
        await Assert.That(FailureReport.Of(outcome.Failure).Message).IsEqualTo("the cause");
    }

    /// <summary>
    /// A consumer's scenarios can live in a namespace that starts the same way this assembly's
    /// does, so the frame filter has to name the framework's own namespaces rather than the root.
    /// </summary>
    [Test]
    public async Task The_line_the_reader_wrote_survives_the_frame_filter()
    {
        var outcome = await Discovered.Named("The last step throws").RunAsync();

        var stackTrace = FailureReport.Of(outcome.Failure).StackTrace;

        await Assert.That(stackTrace).IsNotNull();
        await Assert.That(stackTrace.Contains("Failing_then.a_step_that_throws")).IsTrue();
        await Assert.That(stackTrace.Contains("Behavior.Internal.")).IsFalse();
    }
}
