using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kekiri.Mtp;
using Kekiri.Mtp.Internal;
using Kekiri.Tests.Samples.Setup;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.TestFramework;
using Microsoft.Testing.Platform.Requests;
using TestSessionContext = Microsoft.Testing.Platform.TestHost.TestSessionContext;

namespace Kekiri.Tests;

public class RunnerTests
{
    static (KekiriTestFramework Framework, Platform.Bus Bus, Platform.Output Output) Runner()
    {
        var output = new Platform.Output();
        return (new KekiriTestFramework(typeof(RunnerTests).Assembly, output), new Platform.Bus(), output);
    }

    static ITestExecutionFilter Only(params string[] titles) =>
        new TestNodeUidListFilter(titles
            .Select(t => new TestNodeUid(Discovered.Named(t).Uid))
            .ToArray());

    static async Task<Platform.Completion> Execute(KekiriTestFramework framework, Platform.Bus bus, IRequest request)
    {
        var completion = new Platform.Completion();
        await framework.ExecuteRequestAsync(new ExecuteRequestContext(request, bus, completion, CancellationToken.None));
        return completion;
    }

    [Test]
    public async Task Discovery_publishes_a_node_per_scenario_with_what_an_ide_needs()
    {
        var (framework, bus, _) = Runner();

        var completion = await Execute(framework, bus,
            new DiscoverTestExecutionRequest(Platform.SessionContext(), Only("Carries its tags")));

        var node = bus.Published.Single();
        await Assert.That(completion.Completed).IsTrue();
        await Assert.That(node.Uid.Value).IsEqualTo(Discovered.Named("Carries its tags").Uid);
        await Assert.That(node.Properties.Single<TestNodeStateProperty>()).IsTypeOf<DiscoveredTestNodeStateProperty>();
        await Assert.That(node.Properties.Single<TestMethodIdentifierProperty>().MethodName).IsEqualTo("Carries_its_tags");
        await Assert.That(node.Properties.Single<TestFileLocationProperty>().FilePath.EndsWith("Tagged_scenario.cs")).IsTrue();
        await Assert.That(node.Properties.OfType<TestMetadataProperty>().Select(p => $"{p.Key}={p.Value}").ToArray())
            .IsEquivalentTo(new[] { "Feature=Arithmetic", "Category=smoke", "Category=fast", "Owner=payments" });
    }

    [Test]
    public async Task Discovery_without_a_filter_finds_every_scenario()
    {
        var (framework, bus, _) = Runner();

        await Execute(framework, bus, new DiscoverTestExecutionRequest(Platform.SessionContext()));

        await Assert.That(bus.Published.Count).IsEqualTo(Discovered.All().Count);
    }

    [Test]
    public async Task A_passing_scenario_is_reported_with_its_gherkin_and_timing()
    {
        var (framework, bus, output) = Runner();

        await Execute(framework, bus, new RunTestExecutionRequest(Platform.SessionContext(), Only("Adding 1 and 2")));

        var node = bus.Node("Adding 1 and 2");
        await Assert.That(node.Properties.Single<TestNodeStateProperty>()).IsTypeOf<PassedTestNodeStateProperty>();
        await Assert.That(node.Properties.Single<StandardOutputProperty>().StandardOutput)
            .IsEqualTo(string.Join(Environment.NewLine, "Given a running total", "When adding 1 2", "Then the total is 3"));
        await Assert.That(node.Properties.Single<TimingProperty>().GlobalTiming.Duration).IsGreaterThanOrEqualTo(TimeSpan.Zero);
        await Assert.That(output.Text.Contains("Scenario: Adding 1 and 2")).IsTrue();
    }

    [Test]
    public async Task A_failing_scenario_reports_the_cause()
    {
        var (framework, bus, _) = Runner();

        await Execute(framework, bus, new RunTestExecutionRequest(Platform.SessionContext(), Only("The last step throws")));

        var state = (FailedTestNodeStateProperty)bus.Node("The last step throws").Properties.Single<TestNodeStateProperty>();
        await Assert.That(state.Exception!.Message).IsEqualTo("the cause");
    }

    [Test]
    public async Task A_scenario_with_no_steps_carries_no_output()
    {
        var (framework, bus, _) = Runner();

        await Execute(framework, bus, new RunTestExecutionRequest(Platform.SessionContext(), Only("Has no source location")));

        var node = bus.Node("Has no source location");
        await Assert.That(node.Properties.OfType<StandardOutputProperty>().Any()).IsFalse();
        await Assert.That(node.Properties.OfType<TestFileLocationProperty>().Any()).IsFalse();
    }

    [Test]
    public async Task An_unrecognized_request_is_refused_and_still_completes()
    {
        var (framework, bus, _) = Runner();
        var completion = new Platform.Completion();

        await Assert.ThrowsAsync<NotSupportedException>(() => framework.ExecuteRequestAsync(
            new ExecuteRequestContext(new Unknown_request(), bus, completion, CancellationToken.None)));

        await Assert.That(completion.Completed).IsTrue();
    }

    [Test]
    public async Task The_session_runs_every_setup_once_in_name_order()
    {
        var (framework, _, _) = Runner();
        Setups.Ran.Clear();

        var created = await framework.CreateTestSessionAsync(Platform.CreateSessionContext());
        var closed = await framework.CloseTestSessionAsync(null!);

        await Assert.That(created.IsSuccess).IsTrue();
        await Assert.That(closed.IsSuccess).IsTrue();
        await Assert.That(string.Join(",", Setups.Ran)).IsEqualTo("A_first_setup,B_second_setup");
    }

    [Test]
    public async Task The_runner_describes_itself()
    {
        var (framework, _, _) = Runner();

        await Assert.That(framework.Uid).IsEqualTo("KekiriTestFramework");
        await Assert.That(framework.DisplayName).IsEqualTo("Kekiri");
        await Assert.That(framework.Description.Contains("Microsoft.Testing.Platform")).IsTrue();
        await Assert.That(framework.Version).IsEqualTo(PackageVersion.Current);
        await Assert.That(framework.DataTypesProduced.Single()).IsEqualTo(typeof(TestNodeUpdateMessage));
        await Assert.That(await framework.IsEnabledAsync()).IsTrue();
    }

    [Test]
    [Arguments("1.0.0+abc123", "1.0.0")]
    [Arguments("1.0.0", "1.0.0")]
    public async Task The_reported_version_leaves_out_build_metadata(string informational, string expected)
    {
        await Assert.That(PackageVersion.WithoutBuildMetadata(informational)).IsEqualTo(expected);
    }

    sealed class Unknown_request : IRequest
    {
        public TestSessionContext Session => Platform.SessionContext();
    }
}
