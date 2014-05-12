namespace Kekiri.TestSupport.Scenarios.Exceptions
{
    public class When_fixture_has_private_when_scenario : ScenarioTest
    {
        [Given]
        public void Given() { }

        [When]
        // ReSharper disable once UnusedMember.Local
        private void When() { }

        [Then]
        public void Then() { }
    }
}