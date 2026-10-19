using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Kekiri.Mtp.Internal;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Helpers;
using Microsoft.Testing.Platform.Capabilities;
using Microsoft.Testing.Platform.Capabilities.TestFramework;
using Microsoft.Testing.Platform.Services;

namespace Kekiri.Mtp;

public static class TestApplicationBuilderExtensions
{
    /// <summary>
    /// Registers Kekiri as the test framework. One call, and the test project needs no other
    /// runner: no [Fact]-alike, no adapter package, no VSTest.
    /// </summary>
    public static ITestApplicationBuilder AddKekiri(
        this ITestApplicationBuilder builder,
        Assembly testAssembly = null)
    {
        var assembly = testAssembly ?? Assembly.GetCallingAssembly();

        builder.RegisterTestFramework(
            _ => new KekiriCapabilities(),
            (_, serviceProvider) => new KekiriTestFramework(
                assembly,
                serviceProvider.GetOutputDevice()));

        // Registers --treenode-filter. The platform owns --filter-uid, which is what an IDE
        // sends for "run this test"; this is the human-writable form of the same thing.
#pragma warning disable TPEXP
        builder.AddTreeNodeFilterService(new KekiriExtension());
#pragma warning restore TPEXP

        return builder;
    }

    sealed class KekiriExtension : IExtension
    {
        public string Uid => nameof(Kekiri);

        public string Version => PackageVersion.Current;

        public string DisplayName => "Kekiri";

        public string Description => "Runs Kekiri BDD scenarios natively on Microsoft.Testing.Platform.";

        public Task<bool> IsEnabledAsync() => Task.FromResult(true);
    }

    sealed class KekiriCapabilities : ITestFrameworkCapabilities
    {
        public IReadOnlyCollection<ITestFrameworkCapability> Capabilities { get; } =
            new ITestFrameworkCapability[0];
    }
}
