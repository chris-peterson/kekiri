using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kekiri.Tests;

/// <summary>
/// Drives a fixture the way a runner does: record the scenario's steps by calling its method,
/// then run them. The fixtures under Fixtures carry no [Scenario], so discovery never sees them
/// and a fixture that fails on purpose fails only where a test asked it to.
/// </summary>
static class Run
{
    public static async Task<Result<TFixture>> Scenario<TFixture>(Action<TFixture> scenario)
        where TFixture : ScenarioBase, new()
    {
        var fixture = new TFixture();

        try
        {
            scenario(fixture);
            await fixture.RunAsync();

            return new Result<TFixture>(fixture, null);
        }
        catch (Exception ex)
        {
            return new Result<TFixture>(fixture, ex);
        }
    }

    public sealed class Result<TFixture>
    {
        public Result(TFixture fixture, Exception failure)
        {
            Fixture = fixture;
            Failure = failure;
        }

        public TFixture Fixture { get; }

        public Exception Failure { get; }

        /// <summary>The Gherkin the fixture reported, one line per step.</summary>
        public IReadOnlyList<string> Steps =>
            Fixture is IRecordsSteps recorder ? recorder.Captured.Steps : Array.Empty<string>();
    }
}
