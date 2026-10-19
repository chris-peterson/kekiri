using System.Collections.Generic;
using System.Reflection;
using Autofac;

namespace Kekiri.Tests.Subjects;

/// <summary>Types for the containers under test to find, register, and resolve.</summary>
public interface IGreeter
{
    string Greet();
}

public class Greeter : IGreeter
{
    public string Greet() => "hello";
}

public class Fake_greeter : IGreeter
{
    public string Greet() => "fake";
}

public class Internally_constructed
{
    internal Internally_constructed()
    {
    }
}

public interface IFrom_a_module
{
}

public class From_a_module : IFrom_a_module
{
}

public class Registering_module : Autofac.Module
{
    protected override void Load(ContainerBuilder builder) =>
        builder.RegisterType<From_a_module>().As<IFrom_a_module>();
}

public class Custom_builder
{
    public List<Assembly> Scanned { get; } = new List<Assembly>();

    public IContainer Build(Assembly[] assemblies)
    {
        Scanned.AddRange(assemblies);

        var builder = new ContainerBuilder();
        builder.RegisterInstance<IGreeter>(new Fake_greeter());
        return builder.Build();
    }
}
