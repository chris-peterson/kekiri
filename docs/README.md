# <img src="favicon.svg" alt="Kekiri" width="64" height="64" style="vertical-align: middle"> Kekiri

A .NET framework that supports writing low-ceremony BDD tests using Gherkin language.

Kekiri honors the conventions of the Gherkin [cucumber language](https://cucumber.io/docs/gherkin/reference/).

Coming from Kekiri 5 on xUnit or NUnit? The [migration guide](/migrating) covers moving a suite
to `Kekiri.Mtp`.

## Status

[![build](https://github.com/chris-peterson/kekiri/actions/workflows/ci.yml/badge.svg)](https://github.com/chris-peterson/kekiri/actions/workflows/ci.yml)
[![line coverage](https://img.shields.io/endpoint?url=https%3A%2F%2Fchris-peterson.github.io%2Fkekiri%2Fcoverage%2Flines.json)](/coverage)
[![branch coverage](https://img.shields.io/endpoint?url=https%3A%2F%2Fchris-peterson.github.io%2Fkekiri%2Fcoverage%2Fbranches.json)](/coverage)

| Package | Latest Release |
|:--------|:--------------|
| `Kekiri` | [![NuGet version](https://img.shields.io/nuget/v/Kekiri.svg)](https://www.nuget.org/packages/kekiri) |
| `Kekiri.Mtp` | [![NuGet version](https://img.shields.io/nuget/v/Kekiri.Mtp.svg)](https://www.nuget.org/packages/kekiri.mtp) |
| `Kekiri.IoC.Autofac` | [![NuGet version](https://img.shields.io/nuget/v/Kekiri.IoC.Autofac.svg)](https://www.nuget.org/packages/kekiri.ioc.autofac) |
| `Kekiri.IoC.ServiceProvider` | [![NuGet version](https://img.shields.io/nuget/v/Kekiri.IoC.ServiceProvider.svg)](https://www.nuget.org/packages/kekiri.ioc.serviceprovider) |

## Why Kekiri

Unlike other BDD frameworks that impose process overhead (management of feature files, custom tooling, etc) Kekiri allows developers to write BDD scenarios just as quickly and easily as they would a "plain old" test.

The resulting scenario fixtures are concise, highly portable, and adhere to [Arrange, Act, and Assert](https://automationpanda.com/2020/07/07/arrange-act-assert-a-pattern-for-writing-good-tests/).

IoC is also a first-class citizen encouraging testing object interactions in collaboration rather than isolation. More details on the [wiki](https://github.com/chris-peterson/kekiri/wiki/IoC-Support).

## No test runner underneath

Kekiri is a test framework *on*
[Microsoft.Testing.Platform](https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-intro),
rather than an adapter layered over xUnit or NUnit. MTP is "embedded directly in your test projects,
and there's no other app dependencies, such as `vstest.console` or `dotnet test` needed to run your
tests", so a scenario project references `Kekiri.Mtp` and nothing else, and it builds into an
executable that runs on its own:

```bash
dotnet test                            # the SDK, via global.json
dotnet run --project MyScenarios       # like any other project
./bin/Release/net10.0/MyScenarios      # the built executable, directly
```

All three print the same Gherkin.

That shape is where .NET testing is heading. MTP is
["designed to be the future of test execution in .NET"](https://devblogs.microsoft.com/dotnet/dotnet-test-with-mtp/),
the .NET 10 SDK selects it from `global.json`, and MSTest, NUnit, xUnit.net v3 and TUnit have all
adopted it. The [platform comparison](https://learn.microsoft.com/dotnet/core/testing/test-platforms-overview)
is Microsoft's own guide to picking between the two.
