using System;
using System.Threading;
using System.Threading.Tasks;
using Behavior.Internal.Exceptions;
using Microsoft.Testing.Platform.Extensions.OutputDevice;
using Microsoft.Testing.Platform.OutputDevice;

namespace Behavior.Internal;

/// <summary>
/// Emits the run as Gherkin, in the shape RSpec's documentation formatter uses: the spec text
/// itself is the progress report. The platform's own reporter prints nothing per test at default
/// verbosity, so this becomes the output rather than competing with it.
/// </summary>
sealed class GherkinFormatter
{
    readonly IOutputDevice _output;
    readonly IOutputDeviceDataProducer _producer;

    string _currentFeature;

    public GherkinFormatter(IOutputDevice output, IOutputDeviceDataProducer producer)
    {
        _output = output;
        _producer = producer;
    }

    public async Task ScenarioAsync(Scenario scenario, Scenario.Outcome outcome, TimeSpan elapsed, CancellationToken cancellationToken)
    {
        if (_currentFeature != scenario.FeatureName)
        {
            _currentFeature = scenario.FeatureName;
            // The blank separator is written on its own so a colour code doesn't end up
            // attached to an empty line.
            await WriteAsync(string.Empty, cancellationToken);
            await WriteAsync($"Feature: {scenario.FeatureName}", cancellationToken, Cyan);
        }

        await WriteAsync(string.Empty, cancellationToken);
        await WriteAsync($"  Scenario: {scenario.Title}", cancellationToken);

        var failingStep = FailingStepOf(outcome.Failure);

        foreach (var step in outcome.Steps)
        {
            // Marking the step in place is what makes the failure readable: the reader sees which
            // step broke and why, at the point in the scenario where it happened.
            if (failingStep != null && Names(step, failingStep))
            {
                await WriteAsync($"  {Failed} {step.TrimStart()}", cancellationToken, Red);
                await WriteAsync($"      {FailureReport.OneLine(outcome.Failure)}", cancellationToken, Red);
            }
            else
            {
                await WriteAsync($"    {step}", cancellationToken);
            }
        }

        // A scenario whose Given/When/Then never ran has nothing to show, so say why rather than
        // printing an empty scenario.
        if (outcome.Steps.Count == 0)
        {
            await WriteAsync("    (no steps recorded)", cancellationToken);
        }

        if (outcome.Failure is null)
        {
            await WriteAsync($"    {Passed} passed ({elapsed.TotalMilliseconds:0}ms)", cancellationToken, Green);
        }
        else if (failingStep is null)
        {
            // Failed before or after any step — nothing to mark, so report it on its own line.
            await WriteAsync($"    {Failed} {FailureReport.OneLine(outcome.Failure)}", cancellationToken, Red);
        }
    }

    const string Passed = "✓";
    const string Failed = "✗";

    // Colour goes through the platform's abstraction rather than raw ANSI, so --no-ansi, a
    // redirected stream and CI all still do the right thing without this code knowing.
    static readonly IColor Green = new SystemConsoleColor { ConsoleColor = ConsoleColor.Green };
    static readonly IColor Red = new SystemConsoleColor { ConsoleColor = ConsoleColor.Red };
    static readonly IColor Cyan = new SystemConsoleColor { ConsoleColor = ConsoleColor.Cyan };

    /// <summary>
    /// The wrapper names the step that was running when a Given, When or Then threw. Reading that
    /// beats matching on the message text, which exists to be read rather than parsed.
    /// </summary>
    static string FailingStepOf(Exception failure)
    {
        for (var current = failure; current != null; current = current.InnerException)
        {
            if (current is ScenarioException scenario && !string.IsNullOrEmpty(scenario.StepName))
            {
                return scenario.StepName;
            }
        }

        return null;
    }

    /// <summary>
    /// The step report is Gherkin ("Then the result is 120"); the step name is the method's
    /// prose ("the result is 120"). Compare on the tail so the keyword doesn't matter.
    /// </summary>
    static bool Names(string reportedStep, string stepName) =>
        reportedStep.TrimEnd().EndsWith(stepName.TrimEnd(), StringComparison.OrdinalIgnoreCase);

    Task WriteAsync(string text, CancellationToken cancellationToken, IColor foregroundColor = null) =>
        _output.DisplayAsync(
            _producer,
            new FormattedTextOutputDeviceData(text) { ForegroundColor = foregroundColor },
            cancellationToken);
}
