using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Kekiri.Mtp.Internal;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.OutputDevice;
using Microsoft.Testing.Platform.Extensions.TestFramework;
using Microsoft.Testing.Platform.OutputDevice;
using Microsoft.Testing.Platform.Requests;

namespace Kekiri.Mtp
{
    /// <summary>
    /// SPIKE. Kekiri talking to Microsoft.Testing.Platform directly, with no xUnit or NUnit in the
    /// picture. Discovery and execution were always Kekiri's job — they were just expressed through
    /// a host framework's extensibility points. What owning the TestNode adds is that a scenario's
    /// Gherkin shape reaches the runner, the IDE, and the reports as data rather than as console text.
    /// </summary>
    sealed class KekiriTestFramework : ITestFramework, IDataProducer, IOutputDeviceDataProducer
    {
        readonly Assembly _testAssembly;
        readonly GherkinFormatter _formatter;

        public KekiriTestFramework(Assembly testAssembly, IOutputDevice outputDevice)
        {
            _testAssembly = testAssembly;
            _formatter = new GherkinFormatter(outputDevice, this);
        }

        public string Uid => nameof(KekiriTestFramework);

        public string Version => "0.1.0-spike";

        public string DisplayName => "Kekiri";

        public string Description => "Runs Kekiri BDD scenarios natively on Microsoft.Testing.Platform.";

        public Type[] DataTypesProduced => new[] { typeof(TestNodeUpdateMessage) };

        public Task<bool> IsEnabledAsync() => Task.FromResult(true);

        public Task<CreateTestSessionResult> CreateTestSessionAsync(CreateTestSessionContext context) =>
            Task.FromResult(new CreateTestSessionResult { IsSuccess = true });

        public Task<CloseTestSessionResult> CloseTestSessionAsync(CloseTestSessionContext context) =>
            Task.FromResult(new CloseTestSessionResult { IsSuccess = true });

        public async Task ExecuteRequestAsync(ExecuteRequestContext context)
        {
            try
            {
                switch (context.Request)
                {
                    case DiscoverTestExecutionRequest discover:
                        await DiscoverAsync(context, discover);
                        break;
                    case RunTestExecutionRequest run:
                        await RunAsync(context, run);
                        break;
                    default:
                        throw new NotSupportedException($"Unrecognized request type {context.Request.GetType()}");
                }
            }
            finally
            {
                // Skipping this hangs the test application.
                context.Complete();
            }
        }

        async Task DiscoverAsync(ExecuteRequestContext context, DiscoverTestExecutionRequest request)
        {
            foreach (var scenario in Scenario.DiscoverIn(_testAssembly))
            {
                await context.MessageBus.PublishAsync(this, new TestNodeUpdateMessage(
                    request.Session.SessionUid,
                    new TestNode
                    {
                        Uid = new TestNodeUid(scenario.Uid),
                        DisplayName = scenario.Title,
                        Properties = new PropertyBag(
                            Describe(scenario, DiscoveredTestNodeStateProperty.CachedInstance)),
                    }));
            }
        }

        async Task RunAsync(ExecuteRequestContext context, RunTestExecutionRequest request)
        {
            foreach (var scenario in Scenario.DiscoverIn(_testAssembly))
            {
                var start = DateTimeOffset.UtcNow;
                var stopwatch = Stopwatch.StartNew();
                var outcome = await scenario.RunAsync();
                stopwatch.Stop();

                await _formatter.ScenarioAsync(scenario, outcome, stopwatch.Elapsed, context.CancellationToken);

                // The innermost exception, not Kekiri's wrapper. The wrapper's message names the
                // scenario and the step, which the output already shows, and its stack is five
                // frames of Kekiri internals before reaching the step the reader wrote. Unwrapping
                // puts the cause on the first line and the reader's own code at the top of the stack.
                var state = outcome.Failure is null
                    ? (IProperty)PassedTestNodeStateProperty.CachedInstance
                    : new FailedTestNodeStateProperty(GherkinFormatter.Innermost(outcome.Failure));

                var properties = Describe(scenario, state);

                properties.Add(new TimingProperty(
                    new TimingInfo(start, DateTimeOffset.UtcNow, stopwatch.Elapsed)));

                // The scenario's Gherkin text, attached to the result. DisplayName can't carry it:
                // the terminal reporter escapes newlines, so it renders as one line of literal \n.
                if (outcome.Steps.Count > 0)
                {
                    properties.Add(new StandardOutputProperty(
                        string.Join(Environment.NewLine, outcome.Steps)));
                }

                await context.MessageBus.PublishAsync(this, new TestNodeUpdateMessage(
                    request.Session.SessionUid,
                    new TestNode
                    {
                        Uid = new TestNodeUid(scenario.Uid),
                        DisplayName = scenario.Title,
                        Properties = new PropertyBag(properties),
                    }));
            }
        }

        /// <summary>
        /// The properties every node carries. Feature grouping is a trait rather than a parent node:
        /// a container TestNode gets counted as a test by the reporters, and the ParentTestNodeUid
        /// relationship isn't rendered as a hierarchy today.
        /// </summary>
        static List<IProperty> Describe(Scenario scenario, IProperty state)
        {
            var properties = new List<IProperty>
            {
                state,
                scenario.MethodIdentifier,
                new TestMetadataProperty("Feature", scenario.FeatureName),
            };

            if (scenario.FileLocation is { } location)
            {
                properties.Add(location);
            }

            return properties;
        }
    }
}
