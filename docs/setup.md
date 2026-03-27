# Setup

Kekiri targets `netstandard2.0`. To get started, be sure to have the latest [dotnet core](https://www.microsoft.com/net/core) tools.

## Select Test Runner

### Xunit (recommended)

```bash
PM> Install-Package Kekiri.Xunit
```

### NUnit

```bash
PM> Install-Package Kekiri.NUnit
```

## IoC Integration (optional)

### Autofac

```bash
PM> Install-Package Kekiri.IoC.Autofac
```

Be sure to call `AutofacBootstrapper.Initialize()` before your tests run.

### IServiceProvider

```bash
PM> Install-Package Kekiri.IoC.ServiceProvider
```

Be sure to call `ServiceProviderBootstrapper.Initialize(…)` before your tests run.

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
