# Setup

Kekiri targets `net8.0`. To get started, be sure to have the latest [dotnet](https://www.microsoft.com/net/core) tools.

## Install

```bash
PM> Install-Package Kekiri.Mtp
```

`Kekiri.Mtp` discovers and runs scenarios directly on
[Microsoft.Testing.Platform](https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-intro).
It is the only framework reference a scenario project needs — it depends on `Kekiri` itself, which
holds `Given`/`When`/`Then`, steps, contexts, and the Gherkin reporting — and the project is a
self-executing application:

```xml
<PropertyGroup>
  <OutputType>Exe</OutputType>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="Kekiri.Mtp" Version="1.0.0" />
</ItemGroup>
```

The entry point is generated, so the project needs no `Program.cs`. To write your own, call
`AddKekiri` on the builder:

```csharp
using System.Reflection;
using Kekiri.Mtp;
using Microsoft.Testing.Platform.Builder;

var builder = await TestApplication.CreateBuilderAsync(args);

builder.AddKekiri(Assembly.GetExecutingAssembly());

using var app = await builder.BuildAsync();
return await app.RunAsync();
```

Assertions come from whichever library you prefer — the examples use
[AwesomeAssertions](https://awesomeassertions.org).

A *library* of shared steps or test helpers needs only `Kekiri`. One that references `Kekiri.Mtp`
inherits the MSBuild package that generates the entry point, so `dotnet test` treats it as a test
application of its own and fails on it. Tell it otherwise:

```xml
<IsTestingPlatformApplication>false</IsTestingPlatformApplication>
```

### IDEs

From the CLI a scenario project needs nothing but `Kekiri.Mtp` — `dotnet run` and `dotnet test` both
work. IDE test trees are less settled, and the one lever that moves them is
`Microsoft.NET.Test.Sdk`: it brings no test framework of its own, but it is the only thing in the
graph that sets `IsTestProject=true`, which is the workaround JetBrains names in
[RIDER-129745](https://youtrack.jetbrains.com/issue/RIDER-129745).

What has been observed without it:

| IDE | Without `Microsoft.NET.Test.Sdk` |
|:--|:--|
| Rider | Discovers and runs scenarios |
| VS Code | Showed nothing |
| Visual Studio | Untested |

Referencing it is the configuration that has worked everywhere it was tried. It also means setting
`GenerateProgramFile=false`, since it generates a `Main` of its own for VSTest.

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
PM> Install-Package Kekiri.IoC.Autofac
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
`ConstructorFinder` to any Autofac `IConstructorFinder`; Kekiri filters registrations through the
same finder it activates with, so the two can't disagree.

### IServiceProvider

```bash
PM> Install-Package Kekiri.IoC.ServiceProvider
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

Scenario classes then derive from `TestFixture` (or `TestFixture<TContext>`) directly.

## Naming Conventions

Scenario and step names are read back as prose by the same rules, so either convention works and
both read the same:

| Identifier | Reads as |
|:--|:--|
| `Adding_two_numbers`, `AddingTwoNumbers` | Adding two numbers |
| `Adding_50_and_70`, `Adding50And70` | Adding 50 and 70 |
| `HTTP_is_up`, `HTTPIsUp` | HTTP is up |
| `the_HttpClient_is_ready` | the http client is ready |

A run of capitals is kept as written, digits included, which is what lets a step parameter be named
in its step: `the_user_enters_OPERAND1` with `operand1` set to 12 reads "the user enters 12". A step
starts lowercase, since it follows `Given`, `When`, or `Then`; a leading `Given_`, `When_`, `Then_`,
`And_`, or `But_` on a step method is dropped rather than repeated.

## Scenario Output

A direct run prints every scenario as Gherkin on the console. To also write it to `.feature` files
in the test execution directory, set `KEKIRI_OUTPUT` to contain `files`:

```powershell
$env:KEKIRI_OUTPUT='files'
```

The name of the feature file is based on the containing namespace of the scenario.
For example, if `Adding_two_numbers` was defined in `UnitTests.Features.Addition.Adding_two_numbers`, the output would be written to `Addition.feature`.
