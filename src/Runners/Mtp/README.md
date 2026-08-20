# SPIKE: Kekiri as its own test runner

Not for merge. A working `ITestFramework` on Microsoft.Testing.Platform, so there is no xUnit or
NUnit underneath — the test project references `Kekiri.Mtp` and nothing else.

Deliberately outside `Kekiri.slnx`, so a solution-wide `dotnet test` stays green: the example
includes a scenario that fails on purpose, because terminal reporters only print a test's output
when it fails, and that output is the thing worth looking at.

```bash
dotnet test src/Runners/Mtp/Kekiri.Examples.Mtp/Kekiri.Examples.Mtp.csproj
# or run the executable directly
src/Runners/Mtp/Kekiri.Examples.Mtp/bin/Debug/net8.0/Kekiri.Examples.Mtp --list-tests
```

## What it does

Discovery and execution were always Kekiri's job; they were just expressed through a host
framework's extensibility points. What owning the `TestNode` adds is that a scenario's Gherkin shape
reaches the runner, the reports, and the IDE as *data* instead of as console text:

* `DisplayName` is the scenario title, so the tree reads `Adding 50 and 70` rather than a method name
* the Given/When/Then lines ride along as `StandardOutputProperty`, so a failure shows the scenario,
  not just a stack trace
* `TestMetadataProperty("Feature", …)` is a trait, which is what IDEs group a tree by
* `TestFileLocationProperty` comes from `[CallerFilePath]`/`[CallerLineNumber]` on `[Scenario]`, the
  same trick xUnit v3's `FactAttribute` uses, so navigation needs no PDB reading

The whole runner is four files. `Kekiri.Examples.Mtp` has no `xunit`, no `NUnit`, and no
`Microsoft.NET.Test.Sdk`.

## RSpec-style output

Running the executable directly emits the run as Gherkin, the way RSpec's documentation formatter
emits the spec text:

```
Feature: Addition

  Scenario: A failing scenario reports which step failed
    Given a calculator
      And the user enters 50
    When adding
    Then the result is 120
    ✗ Expected 120 but got 50

  Scenario: Adding 50 and 70
    Given a calculator
      And the user enters 50
      And the user enters 70
    When adding
    Then the result is 120
    ✓ passed (2ms)
```

This works because the platform's own reporter prints nothing per test at default verbosity, so the
Gherkin is the output rather than competing with it. It goes through `IOutputDevice`, the service
extensions use to write to the terminal — not `Console.Write`.

**`dotnet test` swallows it.** The same run under `dotnet test` shows only the platform summary; MTP
substitutes a passthrough output device there, and `--output Detailed` doesn't bring it back (that
flag also collides with `dotnet test`'s own `--output`). So this format is available to a direct run
and a watch loop, not to CI through `dotnet test` as it stands. Whether an IDE shows it is a separate
question: IDEs read the protocol's nodes, not this console stream, which is why the step text is also
attached as `StandardOutputProperty`.

## What the protocol will and won't do

Three findings that shape how far the tree can go, each one learned by trying it:

**A container node is counted as a test.** Publishing a `Feature:` node to group scenarios under made
`--list-tests` report 5 tests for 3 scenarios. Grouping has to be a trait, not a node.

**`ParentTestNodeUid` is not rendered as a hierarchy.** It exists on `TestNodeUpdateMessage`, and the
platform docs describe it as enabling "future enhancements … based on the tree relationship". Today
the terminal reporter flattens it. So a genuine Feature → Scenario → Step tree is a protocol
capability without a consumer yet.

**`DisplayName` can't hold multiple lines.** Newlines come out as literal `␊`. Multi-line content
belongs in `StandardOutputProperty`.

## Not verified

**IDE behavior.** Everything above was checked through the terminal reporter and `--list-tests`. Rider,
Visual Studio, and VS Code each render MTP nodes their own way, and whether they group by the
`Feature` trait or honour the file location is exactly what a spike can't answer from a shell. Open
`Kekiri.Examples.Mtp` in each and look at the test tree.

## What a fuller version needs

* **Per-step timing.** `TimingProperty` takes `StepTimingInfo[]`, which the platform docs call out for
  "a test concept divided into multiple phases" — Given/When/Then exactly. Kekiri times the scenario,
  not the steps, so this needs a step-level hook in the core rather than anything in the runner.
* **A public introspection API.** The runner reads the Gherkin text through
  `InternalsVisibleTo("Kekiri.Mtp")` and an `internal` `IReportTarget`. Real support means a public
  contract for "what steps did this scenario run".
* **Scenario outlines, skip, timeout, parallelism, filtering.** None are implemented. Parallelism and
  filtering are the two that carry design weight: `ITestExecutionFilter` arrives on both requests and
  is currently ignored, so `--filter` does nothing.
* **Plain test methods.** 24 of 95 Kekiri test projects also contain a plain `[Fact]`/`[Theory]`. A
  runner that only understands `[Scenario]` leaves those behind.
