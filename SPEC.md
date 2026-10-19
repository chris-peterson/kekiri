# Kekiri — Specification

Kekiri is a BDD framework for .NET in which scenarios are written as ordinary C# and run
directly on Microsoft.Testing.Platform, with no xUnit or NUnit underneath.

Requirements use [EARS syntax](https://alistairmavin.com/ears) — each is one of:
Ubiquitous (`The <system> shall …`), State-Driven (`While …`), Event-Driven
(`When …`), Optional (`Where …`), or Unwanted Behaviour (`If … then …`).

Each requirement is its own heading carrying a stable ID (`XX-NN`), one level
below its category's heading, so every requirement has a linkable anchor
(`SPEC.md#xx-nn`). Lettered decompositions (`XX-NNa`) each count as one.

## Concepts

- **Scenario** — one Given/When/Then sequence, marked with `[Scenario]`. The unit the platform
  reports as a test.
- **Fixture** — the class a scenario is declared on, deriving from `TestFixture` or
  `TestFixture<TContext>`. A C# grouping, not part of the specification a scenario expresses.
- **Step** — one Given, When, Then, And or But, written as a method on the fixture or as a `Step`
  class reusable across fixtures.
- **Feature** — the Gherkin grouping a scenario belongs to, taken from its containing namespace.
- **Example row** — an `[Example(…)]` on a scenario, binding arguments so the scenario runs once
  per row.
- **Context** — state shared between the steps of one scenario: a typed `TContext` the container
  builds, or a dynamic bag.
- **Tag** — a name/value pair on a scenario or fixture, reported as test metadata and available to
  filters. `[Category]` is shorthand for the `Category` tag.
- **Test application** — the executable a scenario project builds into, which hosts the platform
  and runs the scenarios.

## Requirements

### `SCENARIO`
Authoring surface

#### `SCENARIO-01`
The system shall treat a method marked `[Scenario]` on a fixture as one scenario.

#### `SCENARIO-02`
The system shall run a scenario's steps as Given, then When, then Then, in the order each was
declared.

#### `SCENARIO-03`
The system shall accept a step written as a method on the fixture or as a `Step` class, so that a
step class can be shared by more than one fixture.

#### `SCENARIO-04`
Where a step is asynchronous, the system shall await it before running the next step.

#### `SCENARIO-05`
The system shall share state between the steps of one scenario through a typed context the
container builds, or through a dynamic context where no type is given.

#### `SCENARIO-06`
Where a scenario declares `When(…).Throws()`, the system shall treat the exception as the expected
outcome and make it available to the Thens through `Catch<TException>()`.

#### `SCENARIO-07`
Where a scenario carries `[Example]` rows, the system shall run it once per row, binding each row's
values to the scenario's parameters and substituting them into step names by parameter name.

#### `SCENARIO-08`
If a scenario declares no When or no Then, then the system shall fail that scenario.

### `DISCOVER`
What gets found, and what it is called

#### `DISCOVER-01`
The system shall discover every `[Scenario]` method in the test assembly, and shall not treat a
method without the attribute as a scenario.

#### `DISCOVER-02`
The system shall publish each example row as its own test with its own stable identifier.

#### `DISCOVER-03`
The system shall take a scenario's feature from its fixture's containing namespace.

#### `DISCOVER-04`
The system shall render scenario and step names as prose, accepting both underscore-separated and
Pascal-case identifiers.

#### `DISCOVER-05`
The system shall report each scenario's source file and line, so an IDE can navigate to it without
reading a PDB.

### `RUN`
How a scenario runs

#### `RUN-01`
The system shall run each scenario against a fresh fixture instance.

#### `RUN-02`
When a run starts, the system shall invoke every `IBeforeTestRun` implementation in the test
assembly once, before the first scenario.

#### `RUN-03`
The system shall run scenarios one at a time, in discovery order.

#### `RUN-04`
If a scenario declares parameters but no example row supplies them, then the system shall fail that
scenario when it runs.

### `REPORT`
Gherkin output

#### `REPORT-01`
When a test application is run directly, the system shall print the run as Gherkin — feature,
scenario, and step lines, each scenario carrying its verdict and elapsed time.

#### `REPORT-02`
The system shall emit colour as output-device data rather than as escape sequences, so that a
redirected or non-interactive run carries no escape codes.

#### `REPORT-03`
Where `KEKIRI_OUTPUT` names `files`, the system shall write the run to `.feature` files as well
as to the console.

#### `REPORT-04`
Where `.feature` output is enabled, the system shall write one file per feature, named for that
feature.

### `FAILURE`
What a failure tells the reader

#### `FAILURE-01`
When a step fails, the system shall mark that step in the Gherkin and report the cause beneath it.

#### `FAILURE-02`
The system shall report the innermost exception as the failure, rather than the exception Kekiri
wrapped it in.

#### `FAILURE-03`
Where a failure is Kekiri's own diagnosis that an expected exception was wrong or absent, the
system shall report that diagnosis rather than the exception the scenario asked for.

#### `FAILURE-04`
The system shall omit its own internals and the runtime's reflection frames from a reported stack
trace, and shall omit the trace entirely when no frame remains that the reader wrote.

### `PLATFORM`
Microsoft.Testing.Platform integration

#### `PLATFORM-01`
The system shall register itself as the test framework of the test application, so that a scenario
project needs no other test framework reference.

#### `PLATFORM-02`
The system shall publish a scenario's title as its test node's display name, and its Gherkin step
lines as that node's standard output.

#### `PLATFORM-03`
The system shall express feature and tags as test node metadata rather than as container nodes, so
that the reported test count is the number of scenarios.

#### `PLATFORM-04`
The system shall select scenarios from a tree-node filter naming a feature, a tag, or a scenario
name, and from a request naming specific test node identifiers.

#### `PLATFORM-05`
If any scenario fails, then the test application shall exit with a non-zero status.

### `IOC`
Container integration

#### `IOC-01`
Where a container is bootstrapped, the system shall resolve a scenario's context and step classes
from it.

#### `IOC-02`
The system shall auto-register concrete types from the assemblies the consuming solution builds,
and shall not scan assemblies that arrive as packages.

#### `IOC-03`
Where the code under test ships as a package, the system shall accept an explicit set of assemblies
to scan, and shall allow the default project scan to be turned off.

#### `IOC-04`
Where a consumer opts into non-public constructors, the system shall register and activate types
through the same constructor selection, so that the two cannot disagree.

#### `IOC-05`
The system shall support a `Microsoft.Extensions.DependencyInjection` container built from the
application's own service registrations.

### `PACKAGE`
What ships, and consuming it

#### `PACKAGE-01`
When a project references the `Kekiri.Mtp` package and builds as an executable, the system shall
generate its entry point, so that the project needs no `Program.cs`.

#### `PACKAGE-02`
Where a project writes its own entry point, the system shall register through `AddKekiri` on the
test application builder.

#### `PACKAGE-03`
Where a library references `Kekiri.Mtp` without being a test application itself, the system shall
allow it to opt out of being treated as one.

#### `PACKAGE-04`
The system shall version each published package independently.
