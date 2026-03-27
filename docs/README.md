# <img src="favicon.svg" alt="kekiri" width="64" height="64" style="vertical-align: middle"> kekiri

A .NET framework that supports writing low-ceremony BDD tests using Gherkin language.

Kekiri honors the conventions of the Gherkin [cucumber language](https://cucumber.io/docs/gherkin/reference/).

## Status

[![build](https://github.com/chris-peterson/kekiri/actions/workflows/ci.yml/badge.svg)](https://github.com/chris-peterson/kekiri/actions/workflows/ci.yml)

| Package | Latest Release |
|:--------|:--------------|
| `Kekiri` | [![NuGet version](https://img.shields.io/nuget/dt/Kekiri.svg)](https://www.nuget.org/packages/kekiri) |
| `Kekiri.IoC.Autofac` | [![NuGet version](https://img.shields.io/nuget/dt/Kekiri.IoC.Autofac.svg)](https://www.nuget.org/packages/kekiri.ioc.autofac) |
| `Kekiri.IoC.ServiceProvider` | [![NuGet version](https://img.shields.io/nuget/dt/Kekiri.IoC.ServiceProvider.svg)](https://www.nuget.org/packages/kekiri.ioc.ServiceProvider) |
| `Kekiri.Xunit` | [![NuGet version](https://img.shields.io/nuget/dt/Kekiri.Xunit.svg)](https://www.nuget.org/packages/kekiri.xunit) |
| `Kekiri.NUnit` | [![NuGet version](https://img.shields.io/nuget/dt/Kekiri.NUnit.svg)](https://www.nuget.org/packages/kekiri.nunit) |

## Why Kekiri

Unlike other BDD frameworks that impose process overhead (management of feature files, custom tooling, etc) Kekiri allows developers to write BDD scenarios just as quickly and easily as they would a "plain old" test.

The resulting scenario fixtures are concise, highly portable, and adhere to [Arrange, Act, and Assert](https://automationpanda.com/2020/07/07/arrange-act-assert-a-pattern-for-writing-good-tests/).

IoC is also a first-class citizen encouraging testing object interactions in collaboration rather than isolation. More details on the [wiki](https://github.com/chris-peterson/kekiri/wiki/IoC-Support).
