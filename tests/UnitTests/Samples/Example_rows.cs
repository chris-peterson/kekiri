using System;

namespace Kekiri.Tests.Samples.Rows;

/// <summary>Example rows of every shape a reader can write, and the ways a row can be wrong.</summary>
public class Example_rows : TestFixture
{
    [Scenario]
    [Example(null, "", true, false, new[] { 1, 2 }, 1.5)]
    public void Every_kind_of_value(string missing, string empty, bool yes, bool no, int[] items, double number)
    {
        When(nothing);
        Then(nothing);
    }

    [Scenario]
    [Example(1, 2)]
    public void Converts_to_the_parameter_type(DayOfWeek day, int? count)
    {
        When(nothing);
        Then(nothing);
    }

    [Scenario]
    [Example(typeof(int))]
    public void A_value_that_cannot_convert(int value)
    {
        When(nothing);
        Then(nothing);
    }

    [Scenario]
    [Example(1, 2)]
    public void Too_many_values(int value)
    {
        When(nothing);
        Then(nothing);
    }

    void nothing()
    {
    }
}

public class Unusual_scenarios : ScenarioBase
{
    [Scenario(null)]
    public void Has_no_source_location()
    {
        When(nothing);
        Then(nothing);
    }

    [Scenario]
    public void Generic<T>()
    {
        When(nothing);
        Then(nothing);
    }

    void nothing()
    {
    }
}

public class Cannot_be_created : TestFixture
{
    public Cannot_be_created(int value)
    {
    }

    [Scenario]
    public void Needs_a_constructor_argument()
    {
    }
}
