# Setup

Behavior targets `net8.0`. To get started, be sure to have the latest [dotnet](https://www.microsoft.com/net/core) tools.

## Install

```bash
PM> Install-Package Behavior
```

`Behavior` discovers and runs scenarios directly on
[Microsoft.Testing.Platform](https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-intro).
It is the only framework reference a scenario project needs — it depends on `Behavior` itself, which
holds `Given`/`When`/`Then`, steps, contexts, and the Gherkin reporting — and the project is a
self-executing application:

```xml
<PropertyGroup>
  <OutputType>Exe</OutputType>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="Behavior" Version="0.1.0" />
</ItemGroup>
```

The entry point is generated, so the project needs no `Program.cs`. To write your own, call
`AddBehavior` on the builder:

```csharp
var builder = await TestApplication.CreateBuilderAsync(args);

builder.AddBehavior(Assembly.GetExecutingAssembly());

using var app = await builder.BuildAsync();
return await app.RunAsync();
```

Assertions come from whichever library you prefer — the examples use
[AwesomeAssertions](https://awesomeassertions.org).

A *library* that references `Behavior` — shared steps, test helpers — inherits the MSBuild package
that generates the entry point, so `dotnet test` treats it as a test application of its own and fails
on it. Tell it otherwise:

```xml
<IsTestingPlatformApplication>false</IsTestingPlatformApplication>
```

### Rider

Rider builds its test tree from `IsTestProject`, and a scenario project shows nothing there until
something sets it. Adding `Microsoft.NET.Test.Sdk` is the workaround JetBrains names in
[RIDER-129745](https://youtrack.jetbrains.com/issue/RIDER-129745), and it is what the example
projects here do. It brings no test framework of its own, and running doesn't need it: `dotnet run`
and `dotnet test` both work without it.

`dotnet test` drives the platform directly, which the .NET 10 SDK enables from `global.json`:

```json
{
  "test": {
    "runner": "Microsoft.Testing.Platform"
  }
}
```

Running the executable itself prints the run as Gherkin, in the style of RSpec's documentation
formatter:

```plaintext
Feature: Addition

  Scenario: Adding 50 and 70
    Given a calculator
      And the user enters 50
      And the user enters 70
    When adding
    Then the result is 120
    ✓ passed (2ms)
```

## IoC Integration (optional)

The container has to be built once before any scenario runs, which is what `IBeforeTestRun` is for.

### Autofac

```bash
PM> Install-Package Behavior.Autofac
```

Every concrete type in the assemblies your solution builds is auto-registered, so scenarios can
resolve real collaborators and fake only what they need. Package assemblies are left alone —
registering their types put Autofac's own internals and every transitive dependency in the
container, where one type Autofac couldn't activate failed the whole container.

If the code under test ships as a package rather than as a project in the same solution, name it:

```csharp
AutofacBootstrapper.Initialize(x => x
    .ScanAssembliesOf<SomeServiceType>()            // that type's assembly
    .ScanAssembliesMatching(n => n.StartsWith("Contoso.")));   // a family of packages
```

`ScanProjectAssemblies = false` turns the default off, so only what you name is scanned.

Auto-registration activates through public constructors, and skips a type it finds none on. If your
domain types keep their constructors internal, opt in:

```csharp
AutofacBootstrapper.Initialize(x => x.IncludeNonPublicConstructors());
```

That finds every instance constructor, public or not. For finer control, set
`ConstructorFinder` to any Autofac `IConstructorFinder`; Behavior filters registrations through the
same finder it activates with, so the two can't disagree.

### IServiceProvider

```bash
PM> Install-Package Behavior.ServiceProvider
```

### Bootstrapping

Implement `IBeforeTestRun` on any public class with a parameterless constructor. The runner calls it
once, before the first scenario:

```csharp
public class Bootstrap : IBeforeTestRun
{
    public void Setup()
    {
        AutofacBootstrapper.Initialize();
        // ...or ServiceProviderBootstrapper.Initialize(services);
    }
}
```

Scenario classes then derive from `Scenarios` (or `Scenarios<TContext>`) directly.

## Naming Conventions

Behavior supports both Pascal case conventions (e.g. `WhenDoingTheThing`) as it does
underscore convention (e.g. `When_doing_the_thing`).

## Scenario Output

Behavior supports outputting the cucumber text.
The output settings are controlled via the `BEHAVIOR_OUTPUT` environment variable.

```powershell
$env:BEHAVIOR_OUTPUT='console,files'
```

### Output to Console

To output to the console, ensure that `BEHAVIOR_OUTPUT` contains `console`.

### Output to Files

To output to `.feature` files in the test execution directory, ensure that `BEHAVIOR_OUTPUT` contains `files`.

The name of the feature file is based on the containing namespace of the scenario.
For example, if `Adding_two_numbers` was defined in `UnitTests.Features.Addition.Adding_two_numbers`, the output would be written to `Addition.feature`.
