# Setup

Kekiri targets `net8.0`. To get started, be sure to have the latest [dotnet](https://www.microsoft.com/net/core) tools.

## Select Test Runner

### xUnit (recommended)

```bash
PM> Install-Package Kekiri.Xunit
```

`Kekiri.Xunit` builds on [xUnit.net v3](https://xunit.net/docs/getting-started/v3/migration). It
brings in `xunit.v3.extensibility.core`; reference `xunit.v3` alongside it for `Assert`. A v3 test
project is a self-executing application:

```xml
<PropertyGroup>
  <OutputType>Exe</OutputType>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="Kekiri.Xunit" Version="2.0.0" />
  <PackageReference Include="xunit.v3" Version="4.0.0" />
  <PackageReference Include="xunit.runner.visualstudio" Version="4.0.0" />
</ItemGroup>
```

`dotnet test` drives xUnit v3 through Microsoft.Testing.Platform, which the .NET 10 SDK enables from
`global.json`:

```json
{
  "test": {
    "runner": "Microsoft.Testing.Platform"
  }
}
```

### NUnit

```bash
PM> Install-Package Kekiri.NUnit
```

## IoC Integration (optional)

The container has to be built once before any scenario runs. Both runners have a hook for that, so
no per-fixture attribute is needed.

### Autofac

```bash
PM> Install-Package Kekiri.IoC.Autofac
```

Auto-registration activates through public constructors, and skips a type it finds none on —
registering such a type makes building the whole container throw. If your domain types keep their
constructors internal, opt in:

```csharp
AutofacBootstrapper.Initialize(x => x.IncludeNonPublicConstructors());
```

That finds every instance constructor, public or not. For finer control, set
`ConstructorFinder` to any Autofac `IConstructorFinder`; Kekiri filters registrations through the
same finder it activates with, so the two can't disagree.

### IServiceProvider

```bash
PM> Install-Package Kekiri.IoC.ServiceProvider
```

### Bootstrapping under xUnit

Declare an assembly fixture once. xUnit builds it before the first test in the assembly and disposes
it after the last:

```csharp
[assembly: AssemblyFixture(typeof(Bootstrap))]

public class Bootstrap
{
    public Bootstrap()
    {
        AutofacBootstrapper.Initialize();
        // ...or ServiceProviderBootstrapper.Initialize(services);
    }
}
```

Scenario classes then derive from `Scenarios` (or `Scenarios<TContext>`) directly — they need no
`[Collection]` attribute and no shared base class.

If the container must exist before *discovery* as well as execution, implement
[`ITestPipelineStartup`](https://xunit.net/docs/getting-started/v3/whats-new) instead, which runs
earlier in the pipeline:

```csharp
[assembly: TestPipelineStartup(typeof(Bootstrap))]

public class Bootstrap : ITestPipelineStartup
{
    public ValueTask StartAsync(IMessageSink sink)
    {
        AutofacBootstrapper.Initialize();
        return default;
    }

    public ValueTask StopAsync() => default;
}
```

### Bootstrapping under NUnit

Use a `[SetUpFixture]` with `[OneTimeSetUp]` outside any namespace, which NUnit runs once per
assembly:

```csharp
[SetUpFixture]
public class Bootstrap
{
    [OneTimeSetUp]
    public void Setup() => AutofacBootstrapper.Initialize();
}
```

## Naming Conventions

Kekiri supports both Pascal case conventions (e.g. `WhenDoingTheThing`) as it does
underscore convention (e.g. `When_doing_the_thing`).

## Scenario Output

Kekiri supports outputting the cucumber text.
The output settings are controlled via the `KEKIRI_OUTPUT` environment variable.

```powershell
$env:KEKIRI_OUTPUT='console,files'
```

### Output to Console

To output to the console, ensure that `KEKIRI_OUTPUT` contains `console`.

### Output to Files

To output to `.feature` files in the test execution directory, ensure that `KEKIRI_OUTPUT` contains `files`.

The name of the feature file is based on the containing namespace of the scenario.
For example, if `Adding_two_numbers` was defined in `UnitTests.Features.Addition.Adding_two_numbers`, the output would be written to `Addition.feature`.
