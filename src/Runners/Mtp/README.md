# SPIKE: Kekiri as its own test runner

Not for merge. A working `ITestFramework` on Microsoft.Testing.Platform, so there is no xUnit or
NUnit underneath — the test project's only framework reference is `Kekiri.Mtp`.

Deliberately outside `Kekiri.slnx`, so a solution-wide `dotnet test` stays green: the example
includes a scenario that fails on purpose, because terminal reporters only print a test's output
when it fails, and that output is the thing worth looking at.

```bash
dotnet test src/Runners/Mtp/Kekiri.Examples.Mtp/Kekiri.Examples.Mtp.csproj
# or run the executable directly
src/Runners/Mtp/Kekiri.Examples.Mtp/bin/Debug/net10.0/Kekiri.Examples.Mtp --list-tests
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
* names are rendered as prose, so an `Adding_two_numbers` fixture and an `AddingTwoNumbers` one both
  read `Adding two numbers`

The whole runner is five files. `Kekiri.Examples.Mtp` references no test framework and no VSTest
adapter; it does reference `Microsoft.NET.Test.Sdk`, for the reason under Rider below.

## RSpec-style output

Running the executable directly emits the run as Gherkin, the way RSpec's documentation formatter
emits the spec text:

```
Feature: Addition

  Scenario: A failing scenario reports which step failed
    Given a calculator
      And the user enters 50
    When adding
  ✗ Then the result is 120
      Expected 120 but got 50

  Scenario: Adding 50 and 70
    Given a calculator
      And the user enters 50
      And the user enters 70
    When adding
    Then the result is 120
    ✓ passed (2ms)
```

Feature headers are cyan, a passing verdict green, a failing step and its cause red.

This works because the platform's own reporter prints nothing per test at default verbosity, so the
Gherkin is the output rather than competing with it. It goes through `IOutputDevice`, the service
extensions use to write to the terminal — not `Console.Write`.

Colour is set as `FormattedTextOutputDeviceData.ForegroundColor` rather than by writing ANSI, which
is what lets the platform decide when colour is wanted: piping the run emits no escape codes at all,
and `--no-ansi` and CI are handled without this code knowing about either. Checking it therefore
needs a terminal — `script -q /dev/null <exe>` if you want to see the codes in a captured run.

**`dotnet test` swallows it.** The same run under `dotnet test` shows only the platform summary; MTP
substitutes a passthrough output device there, and `--output Detailed` doesn't bring it back (that
flag also collides with `dotnet test`'s own `--output`). So this format is available to a direct run
and a watch loop, not to CI through `dotnet test` as it stands. Whether an IDE shows it is a separate
question: IDEs read the protocol's nodes, not this console stream, which is why the step text is also
attached as `StandardOutputProperty`.

## Unwrapping the failure

Kekiri wraps a step failure twice over: `ThenFailed` names the step, and `ScenarioException` prefixes
the scenario. Reported as-is, a reader met the wrapper's message and five frames of Kekiri internals
before the `--->` that led to the actual cause.

The runner reports the **innermost** exception instead, and marks the step in place. The scenario and
step names are already on screen, so the exception only has to carry the why:

```
failed A failing scenario reports which step failed (15ms)
  Expected 120 but got 50
    at Kekiri.Examples.Mtp.Addition.Adding_two_numbers.the_result_is_120() in …/Adding_two_numbers.cs:38
```

Frames underneath the step are dropped as well — `System.Reflection.MethodBaseInvoker` and friends,
which are how Kekiri reached the step method rather than anything the reader wrote. xUnit and NUnit
both ship a filter of this shape. It has to be done by handing the reporter a substitute exception
that overrides `StackTrace`, because `FailedTestNodeStateProperty` carries an `Exception` and takes
the trace from it.

Finding which step failed needed a small change in the core: the step name existed only inside the
message text, so `ScenarioException` now carries it as `StepName`. Matching on message text would
have worked until someone reworded a message. Both types are internal, so this adds no public
surface — but it is a change outside the spike directory, and it belongs on the migration branch
rather than here if it's kept.

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

## Rider

Rider discovers and runs the scenarios, renders the namespace/fixture/scenario tree, and shows the
Gherkin and the filtered trace in the output pane. Getting there took one non-obvious package.

**Rider shows nothing for the project without `Microsoft.NET.Test.Sdk`.** That package is the only
thing in the graph that sets `IsTestProject=true`. The Testing Platform capabilities Rider talks
over — `TestingPlatformServer`, `TestContainer`, declared by `Microsoft.Testing.Platform`'s own
targets — already arrive through the `ProjectReference` to `Kekiri.Mtp`, so they were never what was
missing; a two-project probe confirms `IsTestingPlatformApplication` is already `true` without it.
JetBrains names the same package as the workaround in
[RIDER-129745](https://youtrack.jetbrains.com/issue/RIDER-129745).

Referencing it means setting `GenerateProgramFile=false`, since `Program.cs` writes its own `Main`.

The tree comes from the namespace and type, not from the `Feature` trait and not from
`ParentTestNodeUid`. Since those are the only grouping levers an IDE offers, the runner spends them
on Gherkin's own two levels: the feature goes in as the type and the namespace is left empty, so the
tree reads `Addition` → `Adding 50 and 70` and the declaring fixture doesn't appear. A fixture is a
C# artifact rather than part of the spec, and one feature's scenarios are usually spread across
several of them.

Which field Rider reads that from is untested — `TestMethodIdentifierProperty` and `Uid` spell out
the same type, so only one of them has to be right. The tree is the test: `Addition` at the top
means Rider reads the property, and a full `Kekiri.Examples.Mtp.Addition.Adding_two_numbers` path
means it parses the `Uid` and the change belongs there instead.

## Not verified

**Visual Studio and VS Code.** Everything else was checked through Rider, the terminal reporter, and
`--list-tests`. Each IDE renders MTP nodes its own way.

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
