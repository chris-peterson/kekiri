# <img src="favicon.svg" alt="Behavior" width="64" height="64" style="vertical-align: middle"> Behavior

A .NET framework that supports writing low-ceremony BDD tests using Gherkin language.

Behavior honors the conventions of the Gherkin [cucumber language](https://cucumber.io/docs/gherkin/reference/).

It is what [Kekiri](https://www.nuget.org/packages/kekiri) became; the [migration guide](/migrating)
covers moving a suite over.

## Status

[![build](https://github.com/chris-peterson/kekiri/actions/workflows/ci.yml/badge.svg)](https://github.com/chris-peterson/kekiri/actions/workflows/ci.yml)

| Package | Latest Release |
|:--------|:--------------|
| `Behavior` | [![NuGet version](https://img.shields.io/nuget/dt/Behavior.svg)](https://www.nuget.org/packages/behavior) |
| `Behavior.Autofac` | [![NuGet version](https://img.shields.io/nuget/dt/Behavior.Autofac.svg)](https://www.nuget.org/packages/behavior.autofac) |
| `Behavior.ServiceProvider` | [![NuGet version](https://img.shields.io/nuget/dt/Behavior.ServiceProvider.svg)](https://www.nuget.org/packages/behavior.serviceprovider) |

## Why Behavior

Unlike other BDD frameworks that impose process overhead (management of feature files, custom tooling, etc) Behavior allows developers to write BDD scenarios just as quickly and easily as they would a "plain old" test.

The resulting scenario fixtures are concise, highly portable, and adhere to [Arrange, Act, and Assert](https://automationpanda.com/2020/07/07/arrange-act-assert-a-pattern-for-writing-good-tests/).

IoC is also a first-class citizen encouraging testing object interactions in collaboration rather than isolation. More details on the [wiki](https://github.com/chris-peterson/kekiri/wiki/IoC-Support).
