using System.Collections.Generic;
using System.Reflection;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Capabilities;
using Microsoft.Testing.Platform.Capabilities.TestFramework;
using Microsoft.Testing.Platform.Services;

namespace Kekiri.Mtp
{
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

            return builder;
        }

        sealed class KekiriCapabilities : ITestFrameworkCapabilities
        {
            public IReadOnlyCollection<ITestFrameworkCapability> Capabilities { get; } =
                new ITestFrameworkCapability[0];
        }
    }
}
