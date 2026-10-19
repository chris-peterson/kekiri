using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kekiri.Tests.Fixtures;

/// <summary>Each way a scenario can go wrong, and the ways an expected exception is handled.</summary>
public class Failures : TestFixture
{
    public readonly List<string> Calls = new List<string>();

    public InvalidOperationException Caught { get; private set; }

    public void A_given_throws()
    {
        Given(throws);
        When(nothing);
        Then(nothing);
    }

    public void An_async_given_throws_after_awaiting()
    {
        GivenAsync(throws_after_awaiting);
        When(nothing);
        Then(nothing);
    }

    public void The_when_throws_unexpectedly()
    {
        Given(nothing);
        When(throws);
        Then(nothing);
    }

    public void The_when_throws_as_expected()
    {
        When(throws).Throws();
        Then(catches_the_exception);
    }

    public void An_async_when_throws_as_expected()
    {
        WhenAsync(throws_after_awaiting).Throws();
        Then(catches_the_exception);
    }

    public void The_when_throws_as_expected_and_nothing_catches_it()
    {
        When(throws).Throws();
        Then(nothing);
    }

    public void The_when_was_expected_to_throw_and_did_not()
    {
        When(nothing).Throws();
        Then(nothing);
    }

    public void A_then_catches_when_nothing_was_thrown()
    {
        When(nothing);
        Then(catches_the_exception);
    }

    public void A_then_catches_the_wrong_type()
    {
        When(throws).Throws();
        Then(catches_an_argument_exception);
    }

    public void A_then_throws()
    {
        When(nothing);
        Then(throws);
    }

    public void An_async_then_throws_after_awaiting()
    {
        When(nothing);
        ThenAsync(throws_after_awaiting);
    }

    public void There_is_no_when()
    {
        Given(nothing);
        Then(nothing);
    }

    public void There_is_no_then()
    {
        Given(nothing);
        When(nothing);
    }

    public void There_are_two_whens()
    {
        When(nothing);
        When(nothing);
    }

    public void A_step_class_catches_the_exception()
    {
        When(throws).Throws();
        Then<Catching_step>();
    }

    void nothing()
    {
    }

    void throws() => throw new InvalidOperationException("the when broke");

    async Task throws_after_awaiting()
    {
        await Task.Yield();
        throw new InvalidOperationException("the when broke");
    }

    void catches_the_exception() => Caught = Catch<InvalidOperationException>();

    void catches_an_argument_exception() => Catch<ArgumentException>();

    protected override Task BeforeAsync()
    {
        Calls.Add("before");
        return Task.CompletedTask;
    }

    protected override Task AfterAsync()
    {
        Calls.Add("after");
        return Task.CompletedTask;
    }
}

public class Catching_step : Step
{
    public override Task ExecuteAsync()
    {
        Catch<InvalidOperationException>();
        return Task.CompletedTask;
    }
}
