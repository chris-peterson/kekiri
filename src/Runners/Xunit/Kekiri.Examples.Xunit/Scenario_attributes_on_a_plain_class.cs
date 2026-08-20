using Kekiri.Xunit;
using Xunit;

namespace Kekiri.Examples.Xunit
{
    /// <summary>
    /// [Scenario] and [ScenarioOutline] also work on a class that doesn't derive from Scenarios, where
    /// they behave as [Fact] and [Theory]. Tabular tests reach for [Example] this way without wanting a
    /// Given/When/Then chain, and nothing is lost by allowing it: Given/When/Then are protected members
    /// of ScenarioBase, so a class that can't run a scenario can't record one either.
    /// </summary>
    public class Scenario_attributes_on_a_plain_class
    {
        [Scenario]
        public void Scenario_runs_as_a_fact()
        {
            Assert.True(true);
        }

        [ScenarioOutline]
        [Example(1, 2, 3)]
        [Example(20, 5, 25)]
        public void Scenario_outline_runs_as_a_theory(int a, int b, int expected)
        {
            Assert.Equal(expected, a + b);
        }
    }
}
