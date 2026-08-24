namespace Behavior.Tests.Samples.Arithmetic;

public class Adding_numbers : Scenarios
{
    int _total;

    [Scenario]
    public void Adding_1_and_2()
    {
        Given(a_running_total);
        When(adding, 1, 2);
        Then(the_total_is, 3m);
    }

    [Scenario]
    [Example(1, 2, 3)]
    [Example(2, 3, 5)]
    public void Adding_any_two_numbers(decimal first, decimal second, decimal expected)
    {
        Given(a_running_total);
        When(adding, (int)first, (int)second);
        Then(the_total_is, expected);
    }

    void a_running_total() => _total = 0;

    void adding(int first, int second) => _total = first + second;

    void the_total_is(decimal expected)
    {
        if (_total != expected)
        {
            throw new System.Exception($"expected {expected}, was {_total}");
        }
    }
}
