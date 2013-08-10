namespace Kekiri.TestSupport.Scenarios.Exceptions
{
    public class When_fixture_has_private_given_scenario : SuppressedOutputScenarioTest
    {
        // ReSharper disable UnusedMember.Local
        [Given]
        private void GivenThatIsIncorrectlyPrivate()
            // ReSharper restore UnusedMember.Local
        {
        }

        [When]
        public void When()
        {
        }

        [Then]
        public void Then()
        {
        }
    }
}