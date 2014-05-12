namespace Kekiri.TestSupport.Scenarios.Exceptions
{
    public class When_fixture_has_private_then_scenario : ScenarioTest
    {
        [Given]
        public void Given() { }
        [When]
        public void When() { }
        [Then]
        private void Then() { }
    }
}