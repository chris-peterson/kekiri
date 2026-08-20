using System;
using Autofac;
using Autofac.Core.Registration;
using Kekiri.Xunit;
using Xunit;

namespace Kekiri.Examples.Xunit
{
    /// <summary>
    /// Pins what auto-registration reaches. Scanning is limited to the assemblies this solution
    /// builds; package assemblies are left alone. Registering their types put Autofac's own
    /// internals, the test platform, and every transitive dependency in the container, where a
    /// single type Autofac could not activate failed the whole container.
    /// Opt a package back in with ScanAssembliesOf/ScanAssembliesMatching — see
    /// Kekiri.Examples.Xunit.Autofac.Customized.
    /// </summary>
    public class Autofac_scan_scope : Scenarios
    {
        [Scenario]
        public void Types_from_this_solution_are_registered()
        {
            When(resolving_a_type_from_this_assembly);
            Then(it_is_resolved);
        }

        [Scenario]
        public void Types_from_package_assemblies_are_not_registered()
        {
            When(resolving_a_type_from_a_package).Throws();
            Then(it_was_not_registered);
        }

        void resolving_a_type_from_this_assembly()
        {
            Context.Resolved = Container.Resolve<PubliclyConstructed>();
        }

        // Autofac itself arrives as a package. ContainerBuilder is public and concrete, so the old
        // output-directory scan registered it.
        void resolving_a_type_from_a_package()
        {
            Container.Resolve<ContainerBuilder>();
        }

        void it_is_resolved()
        {
            Assert.NotNull(Context.Resolved);
        }

        void it_was_not_registered()
        {
            Assert.IsType<ComponentNotRegisteredException>(Catch<Exception>());
        }
    }
}
