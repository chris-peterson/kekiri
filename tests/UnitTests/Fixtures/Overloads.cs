using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kekiri.Tests.Fixtures;

/// <summary>
/// Every step overload, each recording that it ran. A scenario has one When, so each When
/// overload gets a scenario of its own.
/// </summary>
public class Overloads : TestFixture
{
    public readonly List<string> Calls = new List<string>();

    public Overloads()
    {
        Context.Calls = Calls;
    }

    public void Every_given_and_then()
    {
        Given(zero)
            .And(zero)
            .AndAsync(zero_async)
            .And(one, 1)
            .AndAsync(one_async, 1)
            .And(two, 1, 2)
            .AndAsync(two_async, 1, 2)
            .And(three, 1, 2, 3)
            .AndAsync(three_async, 1, 2, 3)
            .And<Recording_step>("and");
        GivenAsync(zero_async);
        Given(one, 1);
        GivenAsync(one_async, 1);
        Given(two, 1, 2);
        GivenAsync(two_async, 1, 2);
        Given(three, 1, 2, 3);
        GivenAsync(three_async, 1, 2, 3);
        Given<Recording_step>("given");
        When(zero);
        Then(zero)
            .But(zero)
            .ButAsync(zero_async)
            .But(one, 1)
            .ButAsync(one_async, 1)
            .But(two, 1, 2)
            .ButAsync(two_async, 1, 2)
            .But(three, 1, 2, 3)
            .ButAsync(three_async, 1, 2, 3)
            .But<Recording_step>("but");
        ThenAsync(zero_async);
        Then(one, 1);
        ThenAsync(one_async, 1);
        Then(two, 1, 2);
        ThenAsync(two_async, 1, 2);
        Then(three, 1, 2, 3);
        ThenAsync(three_async, 1, 2, 3);
        Then<Recording_step>("then");
    }

    public void When_async() => Only(() => WhenAsync(zero_async));

    public void When_with_one() => Only(() => When(one, 1));

    public void When_async_with_one() => Only(() => WhenAsync(one_async, 1));

    public void When_with_two() => Only(() => When(two, 1, 2));

    public void When_async_with_two() => Only(() => WhenAsync(two_async, 1, 2));

    public void When_with_three() => Only(() => When(three, 1, 2, 3));

    public void When_async_with_three() => Only(() => WhenAsync(three_async, 1, 2, 3));

    public void When_a_step_class() => Only(() => When<Recording_step>("when"));

    void Only(System.Action when)
    {
        Given(nothing);
        when();
        Then(nothing);
    }

    void nothing()
    {
    }

    void zero() => Calls.Add("zero");

    Task zero_async()
    {
        Calls.Add("zero async");
        return Task.CompletedTask;
    }

    void one(int a) => Calls.Add($"one {a}");

    async Task one_async(int a)
    {
        await Task.Yield();
        Calls.Add($"one async {a}");
    }

    void two(int a, int b) => Calls.Add($"two {a} {b}");

    async Task two_async(int a, int b)
    {
        await Task.Yield();
        Calls.Add($"two async {a} {b}");
    }

    void three(int a, int b, int c) => Calls.Add($"three {a} {b} {c}");

    async Task three_async(int a, int b, int c)
    {
        await Task.Yield();
        Calls.Add($"three async {a} {b} {c}");
    }
}

public class Recording_step : Step
{
    readonly string _label;

    public Recording_step(string label)
    {
        _label = label;
    }

    public override Task ExecuteAsync()
    {
        ((List<string>)Context.Calls).Add($"step {_label}");
        return Task.CompletedTask;
    }
}
