using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwesomeAssertions;
using Behavior;

namespace Behavior.Examples.Addition;

public class Adding_numbers_asynchronously : Scenarios
{
    readonly List<int> _numbers = new List<int>();
    int _sum;

    [Scenario]
    public void Every_step_can_be_awaited()
    {
        GivenAsync(a_number, 1)
            .AndAsync(a_number, 2);
        WhenAsync(adding_them_up);
        ThenAsync(the_sum_is, 3);
    }

    [Scenario]
    public void Async_and_sync_steps_mix_in_one_scenario()
    {
        Given(a_number_without_awaiting, 40)
            .AndAsync(a_number, 2);
        WhenAsync(adding_them_up);
        Then(the_sum_is_42);
    }

    async Task a_number(int value)
    {
        await Task.Yield();
        _numbers.Add(value);
    }

    void a_number_without_awaiting(int value) => _numbers.Add(value);

    async Task adding_them_up()
    {
        await Task.Yield();
        _sum = _numbers.Sum();
    }

    async Task the_sum_is(int expected)
    {
        await Task.Yield();
        _sum.Should().Be(expected);
    }

    void the_sum_is_42() => _sum.Should().Be(42);
}
