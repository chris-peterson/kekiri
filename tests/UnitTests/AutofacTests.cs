using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core.Registration;
using Kekiri.IoC.Autofac;
using Kekiri.Tests.Subjects;

namespace Kekiri.Tests;

public class AutofacTests
{
    static AutofacContainer Container(Action<CustomizeBehaviorApi> customize = null)
    {
        var api = new CustomizeBehaviorApi();
        customize?.Invoke(api);

        return new AutofacContainer(new Lazy<IContainer>(() => AutofacContainer.Build(api)));
    }

    [Test]
    public async Task Types_from_this_solution_are_registered()
    {
        using var container = Container();

        await Assert.That(container.Resolve<Greeter>().Greet()).IsEqualTo("hello");
    }

    /// <summary>
    /// Scanning packages registered Autofac's own internals, and one type Autofac can't activate
    /// failed the whole container.
    /// </summary>
    [Test]
    public async Task Types_from_packages_are_not()
    {
        using var container = Container();

        Assert.Throws<ComponentNotRegisteredException>(() => container.Resolve<ContainerBuilder>());

        await Assert.That(container).IsNotNull();
    }

    [Test]
    public async Task Turning_off_the_project_scan_registers_nothing_by_default()
    {
        using var container = Container(x => x.ScanProjectAssemblies = false);

        Assert.Throws<ComponentNotRegisteredException>(() => container.Resolve<Greeter>());

        await Assert.That(container).IsNotNull();
    }

    [Test]
    public async Task A_package_can_be_scanned_by_naming_a_type_in_it()
    {
        using var container = Container(x => x.ScanAssembliesOf<ContainerBuilder>());

        await Assert.That(container.Resolve<ContainerBuilder>()).IsNotNull();
    }

    [Test]
    public async Task A_package_can_be_scanned_by_naming_its_assembly()
    {
        using var container = Container(x => x.ScanAssemblies(typeof(ContainerBuilder).Assembly));

        await Assert.That(container.Resolve<ContainerBuilder>()).IsNotNull();
    }

    [Test]
    public async Task Packages_can_be_scanned_by_name()
    {
        using var container = Container(x => x.ScanAssembliesMatching(name => name == "Autofac"));

        await Assert.That(container.Resolve<ContainerBuilder>()).IsNotNull();
    }

    /// <summary>
    /// The dependency manifest lists libraries that name no assembly to load at run time; they
    /// contribute nothing to scan.
    /// </summary>
    [Test]
    public async Task A_library_with_no_assembly_to_load_is_skipped()
    {
        using var container = Container(x => x
            .ScanAssembliesMatching(name => name == "Microsoft.Testing.Platform.MSBuild"));

        await Assert.That(container.Resolve<Greeter>()).IsNotNull();
    }

    [Test]
    public async Task Internal_constructors_are_ignored_by_default()
    {
        using var container = Container();

        Assert.Throws<ComponentNotRegisteredException>(() => container.Resolve<Internally_constructed>());

        await Assert.That(container).IsNotNull();
    }

    [Test]
    public async Task Internal_constructors_can_be_opted_into()
    {
        using var container = Container(x => x.IncludeNonPublicConstructors());

        await Assert.That(container.Resolve<Internally_constructed>()).IsNotNull();
    }

    [Test]
    public async Task Modules_add_their_registrations()
    {
        using var container = Container(x => x.WithModules(new Registering_module()));

        await Assert.That(container.Resolve<IFrom_a_module>()).IsTypeOf<From_a_module>();
    }

    [Test]
    public async Task A_custom_build_replaces_the_default_one()
    {
        var custom = new Custom_builder();

        using var container = Container(x => x.BuildContainer = custom.Build);

        await Assert.That(container.Resolve<IGreeter>().Greet()).IsEqualTo("fake");
        await Assert.That(custom.Scanned.Contains(typeof(AutofacTests).Assembly)).IsTrue();
    }

    [Test]
    public async Task A_registered_fake_wins_over_the_scanned_type()
    {
        using var container = Container();
        container.Register(new Fake_greeter());

        await Assert.That(container.Resolve<IGreeter>().Greet()).IsEqualTo("fake");
        await Assert.That(container.Resolve<Fake_greeter>().Greet()).IsEqualTo("fake");
    }

    [Test]
    public async Task Each_scenario_gets_its_own_scope_over_one_container()
    {
        var root = new Lazy<IContainer>(() => AutofacContainer.Build(new CustomizeBehaviorApi()));

        using var first = new AutofacContainer(root);
        using var second = new AutofacContainer(root);
        first.Register(new Fake_greeter());

        await Assert.That(first.Resolve<IGreeter>().Greet()).IsEqualTo("fake");
        Assert.Throws<ComponentNotRegisteredException>(() => second.Resolve<IGreeter>());
    }

    [Test]
    public async Task Disposing_ends_the_scope_and_can_be_repeated()
    {
        var container = Container();
        container.Resolve<Greeter>();

        container.Dispose();
        container.Dispose();

        await Assert.That(container).IsNotNull();
    }

    [Test]
    public async Task Initialize_points_scenarios_at_a_container_built_once()
    {
        try
        {
            var built = 0;
            AutofacBootstrapper.Initialize(x => x.BuildContainer = assemblies =>
            {
                built++;
                var builder = new ContainerBuilder();
                builder.RegisterType<Greeter>();
                return builder.Build();
            });

            ScenarioBase.ContainerFactory().Resolve<Greeter>();
            ScenarioBase.ContainerFactory().Resolve<Greeter>();

            await Assert.That(built).IsEqualTo(1);
        }
        finally
        {
            ScenarioBase.ContainerFactory = ContainerSetup.Default;
        }
    }

    [Test]
    public async Task Initialize_again_replaces_the_container()
    {
        try
        {
            AutofacBootstrapper.Initialize(x => x.ScanProjectAssemblies = false);
            Assert.Throws<ComponentNotRegisteredException>(() => ScenarioBase.ContainerFactory().Resolve<Greeter>());

            AutofacBootstrapper.Initialize();

            await Assert.That(ScenarioBase.ContainerFactory().Resolve<Greeter>()).IsNotNull();
        }
        finally
        {
            ScenarioBase.ContainerFactory = ContainerSetup.Default;
        }
    }
}
