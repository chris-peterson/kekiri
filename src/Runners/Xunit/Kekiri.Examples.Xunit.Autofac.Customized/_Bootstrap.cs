using Kekiri.IoC.Autofac;
using Xunit;

[assembly: AssemblyFixture(typeof(Kekiri.Examples.Xunit.Bootstrap))]

namespace Kekiri.Examples.Xunit
{
    /// <summary>
    /// Both Autofac customization knobs at once, so each has coverage the default suite can't give
    /// it: the container is a per-assembly singleton, so a second configuration needs a second
    /// assembly.
    /// </summary>
    public class Bootstrap
    {
        public Bootstrap()
        {
            AutofacBootstrapper.Initialize(x => x
                .IncludeNonPublicConstructors()
                .ScanAssembliesMatching(name => name == "Autofac"));
        }
    }
}
