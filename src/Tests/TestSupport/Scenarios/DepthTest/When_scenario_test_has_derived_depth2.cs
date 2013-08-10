namespace Kekiri.TestSupport.Scenarios.DepthTest
{
    public class When_scenario_test_has_derived_depth2 : When_scenario_test_has_derived_depth1
    {
        [Given]
        public void Given_depth2()
        {
            Levels.Add(ScenarioDepthTestLevel.Depth2);
        }
    }
}