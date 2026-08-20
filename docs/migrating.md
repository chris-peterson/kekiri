# Migrating from Kekiri

Behavior is what Kekiri became. Kekiri ran scenarios *inside* a host test framework: `Kekiri.Xunit`
made every scenario an xUnit test case, `Kekiri.NUnit` made it an NUnit one. `Behavior` runs them
itself, on
[Microsoft.Testing.Platform](https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-intro).

The package ids are new, so an existing suite keeps working until you change them: `Kekiri`,
`Kekiri.Xunit`, `Kekiri.NUnit` and the `Kekiri.IoC.*` packages stay on NuGet at the versions they
are today. Migrating is something you do, not something an update does to you.

Scenario bodies don't change. `Given`/`When`/`Then`, step methods, step classes, typed contexts,
`Throws()` and `Catch<>`, and the IoC packages are all the same code. What changes is the project
file, one namespace, and the handful of attributes that belonged to the host framework rather than
to Behavior.

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
  <PackageReference Include="Behavior" Version="0.1.0" />
</ItemGroup>
```

The scenario project becomes a self-executing application, so the runner package is the only
framework reference it needs — no `Microsoft.NET.Test.Sdk`, no VSTest adapter. The entry point is
generated; you don't write a `Program.cs`. `net8.0` is the floor.

To keep `dotnet test` working, tell the SDK which runner to use, in `global.json`:

```json
{
  "test": {
    "runner": "Microsoft.Testing.Platform"
  }
}
```

## Namespace

```diff
-using Kekiri.Xunit;   // or using Kekiri.NUnit;
+using Behavior;
```

`Scenarios` and `Scenarios<TContext>` keep their names and their members.

## Attributes

| Kekiri | Behavior |
|:--|:--|
| `[Scenario]` | unchanged |
| `[ScenarioOutline]` with `[Example]` rows | `[Scenario]` — the `[Example]` rows are what make it an outline |
| `[Example(…)]` | unchanged |
| xUnit `[Trait("Category", "fast")]` | `[Category("fast")]`, or `[Tag(name, value)]` for any other name |
| NUnit `[Category("fast")]` | `[Category("fast")]` — Behavior's own, same spelling |
| NUnit `[TestFixture]` | drop it; deriving from `Scenarios` is the whole declaration |

## Bootstrapping the container

Kekiri had no run-level hook, so bootstrapping went in a shared base class guarded by a lock:

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

Scenario classes then derive from `Scenarios` directly, and the intermediate base goes away.

## Filtering a run

VSTest's `--filter` no longer selects anything — it isn't the platform's option. Use
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
