using System.Threading.Tasks;

namespace Kekiri.Tests;

/// <summary>
/// https://github.com/chris-peterson/kekiri/issues/34: a lambda's method lives on a
/// compiler-generated closure, not on the fixture, so it has to run against the delegate's own
/// target. Its method name is the compiler's, so the Gherkin reads the lambda's source instead.
/// </summary>
public class LambdaStepTests
{
    [Test]
    [Arguments("A lambda that captures nothing")]
    [Arguments("A lambda that captures its arguments [2, 3]")]
    [Arguments("An async lambda")]
    public async Task A_lambda_step_runs(string scenario)
    {
        var outcome = await Discovered.Named(scenario).RunAsync();

        await Assert.That(outcome.Failure).IsNull();
    }

    [Test]
    public async Task A_lambda_step_reads_as_its_body()
    {
        var outcome = await Discovered.Named("A lambda that captures its arguments [2, 3]").RunAsync();

        await Assert.That(outcome.Steps[0]).IsEqualTo("Given _total = first");
        await Assert.That(outcome.Steps[1]).IsEqualTo("When _total += second");
    }

    [Test]
    public async Task A_block_lambda_reads_as_its_statements_on_one_line()
    {
        var outcome = await Discovered.Named("An async lambda").RunAsync();

        await Assert.That(outcome.Steps[0]).IsEqualTo("Given await Task.Yield(); _total = 1;");
    }

    /// <summary>An empty lambda says nothing, so it reads like a step whose name is only the keyword.</summary>
    [Test]
    public async Task An_empty_lambda_step_is_unnamed()
    {
        var outcome = await Discovered.Named("A lambda that captures nothing").RunAsync();

        await Assert.That(string.Join("|", outcome.Steps)).IsEqualTo("When lambda steps");
    }

    [Test]
    public async Task And_and_but_lambdas_read_as_their_bodies_too()
    {
        var outcome = await Discovered.Named("Joined lambdas").RunAsync();

        await Assert.That(outcome.Failure).IsNull();
        await Assert.That(string.Join("|", outcome.Steps)).IsEqualTo(string.Join("|",
            "Given _total = 1",
            "  And _total++",
            "When _total *= 10",
            "Then _total.ToString()",
            "  But _total.GetHashCode()"));
    }
}
