using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Kekiri.Tests.Subjects;

/// <summary>
/// A third-party container in miniature: the builder a service provider factory hands out, and
/// a record of what was registered through it.
/// </summary>
public class Container_builder
{
    public Container_builder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }

    public List<ServiceDescriptor> Registered { get; } = new List<ServiceDescriptor>();
}

public class Container_builder_factory : IServiceProviderFactory<Container_builder>
{
    public Container_builder Last { get; private set; }

    public Container_builder CreateBuilder(IServiceCollection services) => Last = new Container_builder(services);

    public IServiceProvider CreateServiceProvider(Container_builder builder)
    {
        foreach (var descriptor in builder.Registered)
        {
            builder.Services.Add(descriptor);
        }

        return builder.Services.BuildServiceProvider();
    }
}
