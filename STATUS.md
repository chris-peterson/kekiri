# Kekiri — Spec Coverage Status

Tracking status of the requirements declared in [`SPEC.md`](SPEC.md).
Maintained by `/sextant:spec-status`.

**Last audit:** 2026-09-13
**Spec version:** root SPEC.md (unversioned)
**Coverage:** 38 Covered, 1 Partial, 0 Missing/Contradicts
**Evidence pointers:** symbol

The spec was extracted from this repo's own documentation (`README.md`, `docs/`, and the design
notes in `src/Packages/Kekiri.Mtp/README.md`), so most requirements describe behavior that already
exists. The first audit traced each one against the code rather than against the docs it came from,
which is what found the report-target bug the audit history records.

## Status by category

| Prefix | Count | Status | Notes |
|--------|------:|--------|-------|
| SCENARIO-01..08 | 8 | All Covered | `Internal/ScenarioRunner.cs` (`RunAsync`, `ExpectException`, `Catch`), `ScenarioBase.cs` (`AddStepMethod`, `AddStepClass`), `ScenarioBaseOfT.cs` (`CreateContextObject`), `Internal/Scenario.cs` (`From`, `BindArguments`), `Internal/StepName.cs` (`SubstituteParameters`) |
| DISCOVER-01..05 | 5 | All Covered | `Internal/Scenario.cs` (`DiscoverIn`, `From`, `Uid`, `FeatureName`, `FileLocation`), `Internal/Names.cs` (`Sentence`), `ScenarioAttribute.cs` |
| RUN-01..04 | 4 | All Covered | `Internal/Scenario.cs` (`RunAsync`), `BeforeTestRun.cs` (`InvokeAll`), `KekiriTestFramework.cs` (`CreateTestSessionAsync`, `RunAsync`) |
| REPORT-01..04 | 4 | All Covered | `Internal/GherkinFormatter.cs` (`ScenarioAsync`, `WriteAsync`), `Internal/Reporting/CompositeReportTarget.cs` (`Including`), `Internal/Reporting/FeatureFileReportTarget.cs` (`Report`) |
| FAILURE-01..04 | 4 | All Covered | `Internal/FailureReport.cs` (`Cause`, `Filtered`, `OneLine`), `Internal/GherkinFormatter.cs` (`FailingStepOf`) |
| PLATFORM-01..05 | 5 | 4 Covered, 1 Partial | Covered: `TestApplicationBuilderExtensions.cs` (`AddKekiri`), `KekiriTestFramework.cs` (`RunAsync`, `Describe`), `Internal/ScenarioFilter.cs` (`Apply`). Partial: PLATFORM-05 — the exit code is the platform's own handling of failed nodes; nothing here sets or asserts it, and no scenario in this repo fails on purpose inside an MTP host |
| IOC-01..05 | 5 | All Covered | `Kekiri.IoC.Autofac/AutofacContainer.cs` (scan predicate, `constructorFinder`), `Kekiri.IoC.Autofac/CustomizeBehaviorApi.cs` (`ScanAssembliesOf`, `ScanAssembliesMatching`, `ScanProjectAssemblies`, `IncludeNonPublicConstructors`), `Kekiri.IoC.ServiceProvider/ServiceProviderBootstrapper.cs` (`Initialize`, `UseStartup`) |
| PACKAGE-01..04 | 4 | All Covered | `build/Kekiri.Mtp.props`, `TestingPlatformBuilderHook.cs` (`AddExtensions`), `TestApplicationBuilderExtensions.cs` (`AddKekiri`), `Kekiri.Mtp.csproj` (`IsTestingPlatformApplication`), each package `.csproj` (`Version`) |

## Open / Needs Decision

### REPORT-04 — a `.feature` file is not parseable Gherkin

**The spec says** `.feature` output writes one file per feature, named for that feature. It does,
so the requirement is Covered as written.

**What the file holds** is only the step lines — no `Feature:` header and no `Scenario:` line, so
no Gherkin parser (Pickles, Cucumber) can read it:

```gherkin
Given a calculator
  And the user enters 12
When subtracting
Then the result is 7
```

`Internal/Reporting/ScenarioReportingContext.cs` (`CreateReport`) emits the steps alone, while
`Internal/Config/ConfigFileBasedSettings.cs` defines the `Feature: ` and `Scenario: ` tokens that
nothing consumes. The scenario's title is the blocker: `ScenarioTestMetadata` knows the fixture
type, not the `[Scenario]` method or its example arguments, so emitting a `Scenario:` line means
threading the title from the runner down through the reporting context.

**Resolution is pending a human decision** — thread the title and emit real Gherkin, or narrow the
promise to "the step text, one file per feature" and stop calling the extension `.feature`.

## How to use this file

When you implement a new requirement, change the row's status and add an evidence pointer. When an
audit reveals drift, update the row to **Partial** or **Contradicts** with a one-line note.

Evidence pointers are the file plus its enclosing symbol by default. To use a different
granularity, add `**Evidence pointers:** line` (or `anchor`, `file`, `directory`) to the metadata
block above.

## Audit history

### 2026-09-13 — REPORT-03, REPORT-04 fixed; console output retired

`TestFixture` and `TestFixture<TContext>` now compose their capturing target with the configured
ones rather than replacing them, so `KEKIRI_OUTPUT=files` writes `.feature` files again:
REPORT-03 and REPORT-04 Missing → Covered, coverage 36 → 38.

`KEKIRI_OUTPUT=console` is gone with it. Restoring the path showed it printing the step text a
second time alongside the runner's own Gherkin on a direct run, and `dotnet test` captures stdout
so it reached nobody there either. REPORT-03 reworded to `.feature` files only; `README.md` and
`docs/setup.md` follow.

One needs-decision remains, newly found rather than carried: a `.feature` file holds no `Feature:`
or `Scenario:` line, so it isn't parseable Gherkin.

### 2026-09-13 — First audit (spec-status)

Coverage established at 36 Covered, 1 Partial, 2 Missing across 39 requirements; 2 needs-decision
flagged. REPORT-03 and REPORT-04 are documented and implemented but unreachable — `TestFixture`
overrides the method that builds the configured report targets. PLATFORM-05 is Partial: the
non-zero exit on failure is the platform's, and nothing in this repo exercises it.
