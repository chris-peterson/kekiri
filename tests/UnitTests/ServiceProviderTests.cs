using System;
using System.Linq;
using System.Threading.Tasks;
using Kekiri.IoC;
using Kekiri.Tests.Subjects;
using Kekiri.Tests.WebApp;
using Microsoft.Extensions.DependencyInjection;

namespace Kekiri.Tests;

public class ServiceProviderTests
{
    [Test]
    public async Task Configured_services_are_resolvable()
    {
        var services = new ServiceProviderBootstrapper()
            .ConfigureServices(s => s.AddScoped<IGreeter, Greeter>())
            .BuildServiceProvider();

        await Assert.That(services.GetRequiredService<IGreeter>().Greet()).IsEqualTo("hello");
    }

    [Test]
    public async Task Post_startup_registrations_win()
    {
        var services = new ServiceProviderBootstrapper()
            .ConfigureServices(s => s.AddScoped<IGreeter, Greeter>())
            .ConfigureServicesPostStartup(s => s.AddScoped<IGreeter, Fake_greeter>())
            .BuildServiceProvider();

        await Assert.That(services.GetRequiredService<IGreeter>().Greet()).IsEqualTo("fake");
    }

    [Test]
    public async Task An_assembly_can_override_by_registering_every_concrete_type_in_it()
    {
        var services = new ServiceProviderBootstrapper()
            .ConfigureServices(s => s.AddScoped<IFoo>(_ => null!))
            .OverrideServicesWithTypesFromAssemblyOf<DefaultFoo>()
            .BuildServiceProvider();

        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        await Assert.That(provider.GetRequiredService<IFoo>()).IsTypeOf<DefaultFoo>();
        await Assert.That(provider.GetRequiredService<BarBase>()).IsTypeOf<NullBar>();
        await Assert.That(ReferenceEquals(provider.GetRequiredService<IFoo>(), provider.GetRequiredService<DefaultFoo>())).IsTrue();
    }

    [Test]
    public async Task A_container_builder_can_take_the_post_startup_registrations()
    {
        var factory = new Container_builder_factory();

        var services = new ServiceProviderBootstrapper()
            .ConfigureServices(s => s.AddSingleton<IServiceProviderFactory<Container_builder>>(factory))
            .ConfigureServicesPostStartup<Container_builder>(b =>
                b.Registered.Add(ServiceDescriptor.Scoped<IGreeter, Fake_greeter>()))
            .BuildServiceProvider();

        await Assert.That(services.GetRequiredService<IGreeter>().Greet()).IsEqualTo("fake");
    }

    [Test]
    public async Task A_container_builder_can_register_an_assembly_one_service_at_a_time()
    {
        var factory = new Container_builder_factory();

        var services = new ServiceProviderBootstrapper()
            .ConfigureServices(s => s.AddSingleton<IServiceProviderFactory<Container_builder>>(factory))
            .OverrideServicesWithTypesFromAssemblyUsingContainerBuilder<Container_builder>(
                typeof(DefaultFoo).Assembly,
                (builder, descriptor) => builder.Registered.Add(descriptor))
            .BuildServiceProvider();

        await Assert.That(services.CreateScope().ServiceProvider.GetRequiredService<IFoo>()).IsTypeOf<DefaultFoo>();
    }

    [Test]
    public async Task A_container_builder_can_register_an_assembly_all_at_once()
    {
        var factory = new Container_builder_factory();

        var services = new ServiceProviderBootstrapper()
            .ConfigureServices(s => s.AddSingleton<IServiceProviderFactory<Container_builder>>(factory))
            .OverrideServicesWithTypesFromAssemblyUsingContainerBuilder<Container_builder>(
                typeof(DefaultFoo).Assembly,
                (builder, descriptors) => builder.Registered.AddRange(descriptors))
            .BuildServiceProvider();

        await Assert.That(factory.Last!.Registered.Any(d => d.ServiceType == typeof(IFoo))).IsTrue();
        await Assert.That(services.CreateScope().ServiceProvider.GetRequiredService<IFoo>()).IsTypeOf<DefaultFoo>();
    }

    [Test]
    public async Task A_startup_registers_the_app_services()
    {
        var services = new ServiceProviderBootstrapper()
            .UseStartup<Startup>()
            .BuildServiceProvider();

        using var scope = services.CreateScope();

        await Assert.That(scope.ServiceProvider.GetRequiredService<IBar>()).IsTypeOf<DefaultBar>();
    }

    /// <summary>
    /// The reason UseStartup exists: a test double registered after Startup.ConfigureServices
    /// replaces the app's own registration.
    /// </summary>
    [Test]
    public async Task Post_startup_registrations_win_over_the_startup()
    {
        var services = new ServiceProviderBootstrapper()
            .UseStartup(typeof(Startup))
            .ConfigureServices(s => s.AddScoped<IGreeter, Greeter>())
            .ConfigureServicesPostStartup(s => s.AddScoped<IBar, NullBar>())
            .ConfigureServicesPostStartup<Container_builder>(_ => { })
            .BuildServiceProvider();

        using var scope = services.CreateScope();

        await Assert.That(scope.ServiceProvider.GetRequiredService<IBar>()).IsTypeOf<NullBar>();
        await Assert.That(scope.ServiceProvider.GetRequiredService<IGreeter>()).IsTypeOf<Greeter>();
    }

    [Test]
    public async Task Initialize_points_scenarios_at_the_service_provider()
    {
        var services = new ServiceCollection().AddScoped<IGreeter, Greeter>().BuildServiceProvider();
        try
        {
            ServiceProviderBootstrapper.Initialize(services);

            var container = ScenarioBase.ContainerFactory();

            await Assert.That(container).IsTypeOf<ServiceProviderContainer>();
            await Assert.That(container.Resolve<IGreeter>().Greet()).IsEqualTo("hello");
        }
        finally
        {
            ScenarioBase.ContainerFactory = ContainerSetup.Default;
        }
    }

    [Test]
    public async Task A_scenario_container_resolves_within_one_scope()
    {
        var services = new ServiceCollection().AddScoped<IGreeter, Greeter>().BuildServiceProvider();
        using var container = new ServiceProviderContainer(services);

        await Assert.That(ReferenceEquals(container.Resolve<IGreeter>(), container.Resolve<IGreeter>())).IsTrue();
    }

    [Test]
    public async Task A_scenario_container_creates_what_was_never_registered()
    {
        using var container = new ServiceProviderContainer(() => new ServiceCollection().BuildServiceProvider());

        await Assert.That(container.Resolve<Greeter>().Greet()).IsEqualTo("hello");
    }

    [Test]
    public async Task A_scenario_container_takes_no_registrations()
    {
        using var container = new ServiceProviderContainer(new ServiceCollection().BuildServiceProvider());

        var failure = Assert.Throws<Exception>(() => container.Register(new Fake_greeter()));

        await Assert.That(failure.Message)
            .IsEqualTo("ServiceProviderContainer does not allow just-in-time service registrations");
    }

    [Test]
    public async Task Disposing_a_scenario_container_ends_its_scope()
    {
        var services = new ServiceCollection().AddScoped<IGreeter, Greeter>().BuildServiceProvider();
        var container = new ServiceProviderContainer(services);
        var first = container.Resolve<IGreeter>();

        container.Dispose();
        container.Dispose();

        await Assert.That(ReferenceEquals(container.Resolve<IGreeter>(), first)).IsFalse();
    }
}
