using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Testing.Platform.Extensions.OutputDevice;
using Microsoft.Testing.Platform.OutputDevice;

namespace Kekiri.Mtp.Internal
{
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
                await WriteAsync($"{Environment.NewLine}Feature: {scenario.FeatureName}", cancellationToken);
            }

            await WriteAsync($"{Environment.NewLine}  Scenario: {scenario.Title}", cancellationToken);

            foreach (var step in outcome.Steps)
            {
                await WriteAsync($"    {step}", cancellationToken);
            }

            // A scenario whose Given/When/Then never ran has nothing to show, so say why rather than
            // printing an empty scenario.
            if (outcome.Steps.Count == 0)
            {
                await WriteAsync("    (no steps recorded)", cancellationToken);
            }

            await WriteAsync(
                outcome.Failure is null
                    ? $"    {Passed} passed ({elapsed.TotalMilliseconds:0}ms)"
                    : $"    {Failed} {FirstLineOf(outcome.Failure)}",
                cancellationToken);
        }

        const string Passed = "✓";
        const string Failed = "✗";

        /// <summary>
        /// Kekiri wraps a step failure in its own exception whose first line names the scenario, so
        /// taking the outer message's first line reports what failed and not why. The innermost
        /// exception is the one the reader wants on a single line.
        /// </summary>
        static string FirstLineOf(Exception exception)
        {
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }

            var message = exception.Message;

            if (string.IsNullOrWhiteSpace(message))
            {
                return exception.GetType().Name;
            }

            var newline = message.IndexOfAny(new[] { '\r', '\n' });

            return newline < 0 ? message : message.Substring(0, newline);
        }

        Task WriteAsync(string text, CancellationToken cancellationToken) =>
            _output.DisplayAsync(_producer, new FormattedTextOutputDeviceData(text), cancellationToken);
    }
}
