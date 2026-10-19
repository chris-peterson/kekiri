using System;
using System.Threading.Tasks;
using Kekiri.Internal;
using Kekiri.Internal.Exceptions;
using Kekiri.Tests.Fixtures;

namespace Kekiri.Tests;

public class FailureTests
{
    [Test]
    public async Task A_failing_given_names_its_step()
    {
        var run = await Run.Scenario<Failures>(f => f.A_given_throws());

        await Assert.That(run.Failure).IsTypeOf<GivenFailed>();
        await Assert.That(((ScenarioException)run.Failure).StepName).IsEqualTo("throws");
        await Assert.That(run.Failure.InnerException).IsTypeOf<InvalidOperationException>();
    }

    /// <summary>
    /// An async step that throws after its first await returns a faulted task rather than
    /// throwing through reflection, so there is no TargetInvocationException to unwrap.
    /// </summary>
    [Test]
    public async Task A_given_that_fails_after_awaiting_still_names_its_step()
    {
        var run = await Run.Scenario<Failures>(f => f.An_async_given_throws_after_awaiting());

        await Assert.That(run.Failure).IsTypeOf<GivenFailed>();
        await Assert.That(run.Failure.InnerException!.Message).IsEqualTo("the when broke");
    }

    [Test]
    public async Task A_then_that_fails_after_awaiting_still_names_its_step()
    {
        var run = await Run.Scenario<Failures>(f => f.An_async_then_throws_after_awaiting());

        await Assert.That(run.Failure).IsTypeOf<ThenFailed>();
        await Assert.That(run.Failure.InnerException!.Message).IsEqualTo("the when broke");
    }

    [Test]
    public async Task A_failing_then_names_its_step()
    {
        var run = await Run.Scenario<Failures>(f => f.A_then_throws());

        await Assert.That(run.Failure).IsTypeOf<ThenFailed>();
        await Assert.That(((ScenarioException)run.Failure).StepName).IsEqualTo("throws");
    }

    [Test]
    public async Task An_unexpected_exception_from_the_when_says_how_to_expect_it()
    {
        var run = await Run.Scenario<Failures>(f => f.The_when_throws_unexpectedly());

        await Assert.That(run.Failure).IsTypeOf<WhenFailed>();
        await Assert.That(run.Failure.Message.Contains("use .Throws()")).IsTrue();
    }

    [Test]
    public async Task An_expected_exception_is_handed_to_the_step_that_catches_it()
    {
        var run = await Run.Scenario<Failures>(f => f.The_when_throws_as_expected());

        await Assert.That(run.Failure).IsNull();
        await Assert.That(run.Fixture.Caught!.Message).IsEqualTo("the when broke");
    }

    [Test]
    public async Task An_async_when_can_throw_as_expected()
    {
        var run = await Run.Scenario<Failures>(f => f.An_async_when_throws_as_expected());

        await Assert.That(run.Failure).IsNull();
        await Assert.That(run.Fixture.Caught!.Message).IsEqualTo("the when broke");
    }

    [Test]
    public async Task A_step_class_can_catch_the_expected_exception()
    {
        var run = await Run.Scenario<Failures>(f => f.A_step_class_catches_the_exception());

        await Assert.That(run.Failure).IsNull();
    }

    [Test]
    public async Task An_expected_exception_nobody_catches_fails_the_scenario()
    {
        var run = await Run.Scenario<Failures>(f => f.The_when_throws_as_expected_and_nothing_catches_it());

        await Assert.That(run.Failure).IsTypeOf<ExpectedExceptionNotCaught>();
        await Assert.That(FailureReport.OneLine(run.Failure))
            .IsEqualTo("Expected InvalidOperationException, missing Catch<InvalidOperationException>()");
    }

    [Test]
    public async Task Expecting_an_exception_that_never_comes_fails_the_scenario()
    {
        var run = await Run.Scenario<Failures>(f => f.The_when_was_expected_to_throw_and_did_not());

        await Assert.That(run.Failure).IsTypeOf<NoExceptionThrown>();
        await Assert.That(FailureReport.OneLine(run.Failure))
            .IsEqualTo("Expected an exception, but nothing was thrown");
    }

    [Test]
    public async Task Catching_when_nothing_was_thrown_names_the_type_it_wanted()
    {
        var run = await Run.Scenario<Failures>(f => f.A_then_catches_when_nothing_was_thrown());

        await Assert.That(FailureReport.OneLine(run.Failure))
            .IsEqualTo("Expected InvalidOperationException, but nothing was thrown");
    }

    [Test]
    public async Task Catching_the_wrong_type_names_both_types()
    {
        var run = await Run.Scenario<Failures>(f => f.A_then_catches_the_wrong_type());

        await Assert.That(FailureReport.OneLine(run.Failure))
            .IsEqualTo("Expected ArgumentException, but was InvalidOperationException");
    }

    [Test]
    public async Task A_scenario_needs_a_when()
    {
        var run = await Run.Scenario<Failures>(f => f.There_is_no_when());

        await Assert.That(run.Failure).IsTypeOf<FixtureShouldHaveWhens>();
    }

    [Test]
    public async Task A_scenario_needs_a_then()
    {
        var run = await Run.Scenario<Failures>(f => f.There_is_no_then());

        await Assert.That(run.Failure).IsTypeOf<FixtureShouldHaveThens>();
    }

    [Test]
    public async Task A_scenario_has_one_when()
    {
        var run = await Run.Scenario<Failures>(f => f.There_are_two_whens());

        await Assert.That(run.Failure).IsTypeOf<NotSupportedException>();
        await Assert.That(run.Failure.Message.StartsWith("Only a single 'When' is supported")).IsTrue();
    }

    [Test]
    public async Task After_runs_even_when_the_scenario_fails()
    {
        var run = await Run.Scenario<Failures>(f => f.A_then_throws());

        await Assert.That(string.Join(",", run.Fixture.Calls)).IsEqualTo("before,after");
    }

    [Test]
    public async Task Initialize_starts_the_fixture_over()
    {
        var fixture = new Failures();
        fixture.A_given_throws();
        fixture.Initialize();
        fixture.The_when_throws_as_expected();

        await fixture.RunAsync();

        await Assert.That(fixture.Caught).IsNotNull();
    }
}
