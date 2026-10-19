# Kekiri

Low-ceremony BDD scenarios in C#, written as ordinary code and run directly on
[Microsoft.Testing.Platform](https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-intro).
Scenarios follow the conventions of the Gherkin
[cucumber language](https://cucumber.io/docs/gherkin/reference/), with no feature files or custom
tooling to maintain.

[![build](https://github.com/chris-peterson/kekiri/actions/workflows/ci.yml/badge.svg)](https://github.com/chris-peterson/kekiri/actions/workflows/ci.yml)
[![line coverage](https://img.shields.io/endpoint?url=https%3A%2F%2Fchris-peterson.github.io%2Fkekiri%2Fcoverage%2Flines.json)](https://chris-peterson.github.io/kekiri/#/coverage)
[![branch coverage](https://img.shields.io/endpoint?url=https%3A%2F%2Fchris-peterson.github.io%2Fkekiri%2Fcoverage%2Fbranches.json)](https://chris-peterson.github.io/kekiri/#/coverage)

| Package | Latest release |
|:--|:--|
| `Kekiri` | [![NuGet](https://img.shields.io/nuget/v/Kekiri.svg)](https://www.nuget.org/packages/Kekiri) |
| `Kekiri.Mtp` | [![NuGet](https://img.shields.io/nuget/v/Kekiri.Mtp.svg)](https://www.nuget.org/packages/Kekiri.Mtp) |
| `Kekiri.IoC.Autofac` | [![NuGet](https://img.shields.io/nuget/v/Kekiri.IoC.Autofac.svg)](https://www.nuget.org/packages/Kekiri.IoC.Autofac) |
| `Kekiri.IoC.ServiceProvider` | [![NuGet](https://img.shields.io/nuget/v/Kekiri.IoC.ServiceProvider.svg)](https://www.nuget.org/packages/Kekiri.IoC.ServiceProvider) |

## A scenario

```csharp
namespace Addition;

public class Adding_two_numbers : TestFixture
{
    readonly Calculator _calculator = new Calculator();

    [Scenario]
    public void Adding_50_and_70()
    {
        Given(the_first_number_is, 50m)
            .And(the_second_number_is, 70m);
        When(adding);
        Then(the_result_is, 120m);
    }

    void the_first_number_is(decimal number) => _calculator.Operand1 = number;
    void the_second_number_is(decimal number) => _calculator.Operand2 = number;
    void adding() => _calculator.Add();
    void the_result_is(decimal expected) => _calculator.Result.Should().Be(expected);
}
```

A scenario project references `Kekiri.Mtp` and builds into an executable, so `dotnet test`,
`dotnet run`, and the built binary all run it and print the run as Gherkin:

```plaintext
Feature: Addition

  Scenario: Adding 50 and 70
    Given the first number is 50
      And the second number is 70
    When adding
    Then the result is 120
    ✓ passed (2ms)
```

## Documentation

The [docs site](https://chris-peterson.github.io/kekiri/) covers using Kekiri:

* [Setup](https://chris-peterson.github.io/kekiri/#/setup): installing, IDEs, IoC integration, output
* [Examples](https://chris-peterson.github.io/kekiri/#/examples): expected exceptions, scenario outlines, tags
* [Migrating to Kekiri 6](https://chris-peterson.github.io/kekiri/#/migrating): from `Kekiri.Xunit` or `Kekiri.NUnit`
* [Coverage](https://chris-peterson.github.io/kekiri/#/coverage): the current report

## Development

Building needs the .NET 10 SDK and the .NET 8 runtime the packages target.

```bash
dotnet build
dotnet test
python3 scripts/coverage.py    # tests with coverage, and the 90% gate CI enforces
```

| Path | Holds |
|:--|:--|
| `src/Packages/Kekiri` | The engine: `Given`/`When`/`Then`, steps, contexts, reporting |
| `src/Packages/Kekiri.Mtp` | The runner on Microsoft.Testing.Platform; its [README](src/Packages/Kekiri.Mtp/README.md) has the design notes |
| `src/Packages/Kekiri.IoC.*` | Autofac and `IServiceProvider` integration |
| `tests/UnitTests` | The TUnit suite coverage is measured from |
| `tests/WebApp` | An ASP.NET Core startup the `IServiceProvider` tests host |

Kekiri is tested with TUnit rather than with itself, so a defect in the runner can't hide the
failures that would reveal it.

Every C# block in this README, `docs/`, and the package READMEs is compiled against the current
packages by `DocSnippetTests`, and the scenarios in it run. A block that is a fragment, or code from
a previous version, carries `<!-- snippet: skip -->` on the line above its fence; one whose scenario
fails on purpose carries `<!-- snippet: compile-only -->`.

## Native AOT

Not supported, and not currently worth pursuing. Recorded here so it doesn't get re-investigated
per-project.

Three things block it independently, so fixing any one of them changes nothing on its own:

1. **The untyped `Context` is `dynamic`.** Every `Context.Foo = ...` in a scenario compiles to a
   call site on the C# runtime binder, which needs runtime code generation. This is the documented way
   to write an untyped scenario, so removing it means removing a first-class feature (or reshaping
   `Context` into an indexer, which breaks every existing untyped scenario).
2. **`Kekiri.IoC.Autofac` is assembly scanning, by design.** It reads `DependencyContext.Default`,
   loads assemblies by name, and hands them to Autofac's `RegisterAssemblyTypes`.
   `DependencyContext.Default` is documented as returning null for an app published as a single file,
   and Autofac's scanning entry points carry `[RequiresUnreferencedCode]` because the trimmer cannot
   know which types to keep. Native AOT always trims, so the package cannot keep its behavior and be
   AOT-safe.
3. **Discovery and step invocation are reflection.** The runner enumerates the test assembly's
   types, reads attributes, and calls step methods through `MethodInfo.Invoke`. Under AOT the trimmer
   cannot know which of those to keep, so discovery would have to move to a source generator.

Independently, `Moq` relies on `Reflection.Emit` through Castle DynamicProxy, so a test project using
mocks is out regardless of what Kekiri does.

An AOT-capable subset would be `TestFixture<TContext>` plus `Kekiri.IoC.ServiceProvider` without
`UseStartup`, behind a source generator for discovery — that is, giving up untyped scenarios and
Autofac. If that combination ever becomes worth supporting, start by setting `IsAotCompatible` on the
libraries to get the real analyzer output rather than working from this list.

Worth doing regardless of AOT, because they cost nothing and remove reflection from paths that don't
need it:

* `Kekiri.IoC.ServiceProvider`'s `UseStartup` reconstructs a generic call through
  `MakeGenericMethod`; capturing a delegate while the type argument is still known statically removes
  it, the same way `ConfigureTestContainer` already does.
* `ScenarioBase<TContext>` resolves its context through `MakeGenericMethod` on `Container.Resolve`.
  Constraining `TContext` to a reference type makes it a direct `Container.Resolve<TContext>()` call.
* Two binder call sites on the *typed* path (the `Context` overrides in `ScenarioBase<TContext>` and
  `Step<TContext>`) pull the runtime binder into scenarios that are otherwise fully typed. Casting
  from an `object` backing field removes them with no public API change.
* `FeatureFileReportTarget` keeps a `Dictionary<string, dynamic>` where a `string` value would do.

None of the above has been checked against an actual `PublishAot` run; it comes from reading the code
and the AOT documentation.

## References

Kekiri runs on Microsoft.Testing.Platform:

* [Overview](<https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-intro>) — what the platform is and how a test project runs on it
* [Build a test framework](<https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-architecture-test-framework>) — the `ITestFramework` surface Kekiri implements to own discovery and execution
* [Build extensions](<https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-architecture-extensions>) — the extension points the platform hands a framework
* [CLI options](<https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-cli-options>) — `--treenode-filter`, `--filter-uid`, and the rest of what a run accepts
