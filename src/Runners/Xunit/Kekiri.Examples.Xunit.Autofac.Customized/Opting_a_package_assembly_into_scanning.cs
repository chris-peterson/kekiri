using Autofac;
using Kekiri.Xunit;
using Xunit;

namespace Kekiri.Examples.Xunit
{
    /// <summary>
    /// The other half of Kekiri.Examples.Xunit's Autofac_scan_scope: package assemblies are skipped
    /// by default, and ScanAssembliesMatching pulls a named one back in. Needed when the code under
    /// test ships as a package rather than as a project in the same solution.
    /// </summary>
    public class Opting_a_package_assembly_into_scanning : Scenarios
    {
        [Scenario]
        public void A_type_from_the_named_package_is_registered()
        {
            When(resolving_a_type_from_the_named_package);
            Then(it_is_resolved);
        }

        void resolving_a_type_from_the_named_package()
        {
            Context.Resolved = Container.Resolve<ContainerBuilder>();
        }

        void it_is_resolved()
        {
            Assert.NotNull(Context.Resolved);
        }
    }
}
