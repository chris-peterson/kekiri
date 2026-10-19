using System;
using System.Threading.Tasks;

namespace Kekiri.Tests.Samples.Lambdas;

public class Lambda_steps : TestFixture
{
    int _total;

    [Scenario]
    public void A_lambda_that_captures_nothing()
    {
        Given(() => { });
        When(() => { });
        Then(() => { });
    }

    [Scenario]
    [Example(2, 3)]
    public void A_lambda_that_captures_its_arguments(int first, int second)
    {
        Given(() => _total = first);
        When(() => _total += second);
        Then(() =>
        {
            if (_total != first + second)
            {
                throw new Exception($"expected {first + second}, was {_total}");
            }
        });
    }

    [Scenario]
    public void Joined_lambdas()
    {
        Given(() => _total = 1).And(() => _total++);
        When(() => _total *= 10);
        Then(() => _total.ToString()).But(() => _total.GetHashCode());
    }

    [Scenario]
    public void An_async_lambda()
    {
        GivenAsync(async () => { await Task.Yield(); _total = 1; });
        WhenAsync(async () => { await Task.Yield(); _total++; });
        ThenAsync(async () =>
        {
            await Task.Yield();
            if (_total != 2)
            {
                throw new Exception($"expected 2, was {_total}");
            }
        });
    }
}
