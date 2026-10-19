# Migrating to Kekiri 6

Kekiri 5 ran scenarios *inside* a host test framework: `Kekiri.Xunit` made every scenario an xUnit
test case and `Kekiri.NUnit` made it an NUnit one. Kekiri 6 is the convention and the runner both,
on [Microsoft.Testing.Platform](https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-intro).
`Kekiri.Mtp` is that runner, and it replaces both host-framework packages.

| Kekiri 5 | Kekiri 6 |
|:--|:--|
| `Kekiri` 5.x | `Kekiri` 6.0 |
| `Kekiri.Xunit` 1.x | `Kekiri.Mtp` 1.0 |
| `Kekiri.NUnit` 2.x | `Kekiri.Mtp` 1.0 |
| `Kekiri.IoC.Autofac` 4.x | `Kekiri.IoC.Autofac` 5.0 |
| `Kekiri.IoC.ServiceProvider` 1.x | `Kekiri.IoC.ServiceProvider` 2.0 |

Scenario bodies don't change. `Given`/`When`/`Then`, step methods, step classes, typed contexts,
`Throws()` and `Catch<>`, and the IoC bootstrappers are all the same code. What changes is the
project file, the base class name, and the handful of attributes that belonged to the host framework
rather than to Kekiri.

## Project file

Before, with xUnit:

```xml
<ItemGroup>
  <PackageReference Include="Kekiri.Xunit" Version="1.2.0" />
  <PackageReference Include="xunit" Version="2.4.*" />
  <PackageReference Include="xunit.runner.visualstudio" Version="2.4.*" />
  <PackageReference Include="Microsoft.NET.Test.Sdk" Version="16.7.*" />
</ItemGroup>
```

After:

```xml
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
  <OutputType>Exe</OutputType>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="Kekiri.Mtp" Version="1.0.0" />
</ItemGroup>
```

The scenario project becomes a self-executing application, so the runner package is the only
framework reference it needs: no VSTest adapter, and nothing that brings a test framework of its
own. The entry point is generated; you don't write a `Program.cs`. `net8.0` is the floor.
`Microsoft.NET.Test.Sdk` is the one thing worth keeping in some editors; see
[setup](https://chris-peterson.github.io/kekiri/#/setup) for which.

To keep `dotnet test` working, tell the SDK which runner to use, in `global.json`:

```json
{
  "test": {
    "runner": "Microsoft.Testing.Platform"
  }
}
```

## Namespace and base class

```diff
-using Kekiri.Xunit;   // or using Kekiri.NUnit;
+using Kekiri;
```

`Scenarios` is now `TestFixture`, and `Scenarios<TContext>` is `TestFixture<TContext>`. Their members are
unchanged.

## Attributes

| Kekiri 5 | Kekiri 6 |
|:--|:--|
| `[Scenario]` | unchanged |
| `[ScenarioOutline]` with `[Example]` rows | `[Scenario]`; the `[Example]` rows are what make it an outline |
| `[Example(…)]` | unchanged |
| xUnit `[Trait("Category", "fast")]` | `[Category("fast")]`, or `[Tag(name, value)]` for any other name |
| NUnit `[Category("fast")]` | `[Category("fast")]`, Kekiri's own, same spelling |
| NUnit `[TestFixture]` | drop it; deriving from Kekiri's `TestFixture` is the whole declaration |

## Bootstrapping the container

Kekiri 5 had no run-level hook, so bootstrapping went in a shared base class guarded by a lock:

<!-- snippet: skip -->
```csharp
public class ExampleScenarios : Scenarios
{
    protected override Task BeforeAsync() => BootstrapHelper.EnsureBootstrapped();
}
```

...or, on NUnit, in a `[SetUpFixture]` with `[OneTimeSetUp]`. Both are replaced by `IBeforeTestRun`,
which the runner calls once before the first scenario:

```csharp
public class Bootstrap : IBeforeTestRun
{
    public void Setup() => AutofacBootstrapper.Initialize();
}
```

Scenario classes then derive from `TestFixture` directly, and the intermediate base goes away.

## How steps read

A step name reads by the same rules as a scenario name, so a few read differently than they did in
Kekiri 5. Nothing about which steps run changes, only the Gherkin text:

| Step | Kekiri 5 | Kekiri 6 |
|:--|:--|:--|
| `the_HttpClient_is_ready` | the HttpClient is ready | the http client is ready |
| `Adding50And70` | adding50 and70 | adding 50 and 70 |
| `HTTPIsUp` | HTTP Is Up | HTTP is up |
| `.But(nothing_is_saved)` | And nothing is saved | But nothing is saved |

## Filtering a run

VSTest's `--filter` no longer selects anything; it isn't the platform's option. Use
`--treenode-filter`, which reads the tree the IDE shows:

```bash
dotnet test --treenode-filter '/**[Category=fast]'
dotnet test --treenode-filter '/**[Feature=Subtraction]'
dotnet run --treenode-filter '/*/*/*/Adding_50_and_70'
```

## What has no equivalent

* **Skipping.** xUnit's `[Scenario(Skip = "…")]` came from `FactAttribute`, and NUnit's `[Ignore]`
  from NUnit; neither has a replacement yet.
* **Parallel execution.** Scenarios run one at a time. xUnit ran test classes in parallel by default,
  so a large suite takes longer.
* **Mixing frameworks in one project.** A test application registers a single test framework, so
  plain `[Fact]` or `[Test]` methods alongside your scenarios are no longer discovered. Move them to
  their own project.
