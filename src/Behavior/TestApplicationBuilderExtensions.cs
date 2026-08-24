using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Behavior.Internal;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Helpers;
using Microsoft.Testing.Platform.Capabilities;
using Microsoft.Testing.Platform.Capabilities.TestFramework;
using Microsoft.Testing.Platform.Services;

namespace Behavior;

public static class TestApplicationBuilderExtensions
{
    /// <summary>
    /// Registers Behavior as the test framework. One call, and the test project needs no other
    /// runner: no [Fact]-alike, no adapter package, no VSTest.
    /// </summary>
    public static ITestApplicationBuilder AddBehavior(
        this ITestApplicationBuilder builder,
        Assembly testAssembly = null)
    {
        var assembly = testAssembly ?? Assembly.GetCallingAssembly();

        builder.RegisterTestFramework(
            _ => new BehaviorCapabilities(),
            (_, serviceProvider) => new BehaviorTestFramework(
                assembly,
                serviceProvider.GetOutputDevice()));

        // Registers --treenode-filter. The platform owns --filter-uid, which is what an IDE
        // sends for "run this test"; this is the human-writable form of the same thing.
#pragma warning disable TPEXP
        builder.AddTreeNodeFilterService(new BehaviorExtension());
#pragma warning restore TPEXP

        return builder;
    }

    sealed class BehaviorExtension : IExtension
    {
        public string Uid => nameof(Behavior);

        public string Version => PackageVersion.Current;

        public string DisplayName => "Behavior";

        public string Description => "Runs Behavior BDD scenarios natively on Microsoft.Testing.Platform.";

        public Task<bool> IsEnabledAsync() => Task.FromResult(true);
    }

    sealed class BehaviorCapabilities : ITestFrameworkCapabilities
    {
        public IReadOnlyCollection<ITestFrameworkCapability> Capabilities { get; } =
            new ITestFrameworkCapability[0];
    }
}
