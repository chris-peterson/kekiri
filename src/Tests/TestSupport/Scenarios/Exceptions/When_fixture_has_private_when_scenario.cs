namespace Kekiri.TestSupport.Scenarios.Exceptions
{
    public class When_fixture_has_private_when_scenario : SuppressedOutputScenarioTest
    {
        [Given]
        public void Given() { }

        // ReSharper disable UnusedMember.Local
        [When]
        private void When() { }
        // ReSharper restore UnusedMember.Local

        [Then]
        public void Then() { }
    }
}