namespace Kekiri.TestSupport.Scenarios.DepthTest
{
    public class When_scenario_test_has_derived_depth1 : ScenarioDepthTestBase
    {
        [Given]
        public void Given_depth1()
        {
            Levels.Add(ScenarioDepthTestLevel.Depth1);
        }
    }
}