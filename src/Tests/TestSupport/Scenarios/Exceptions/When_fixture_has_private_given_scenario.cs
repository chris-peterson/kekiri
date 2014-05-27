namespace Kekiri.TestSupport.Scenarios.Exceptions
{
    public class When_fixture_has_private_given_scenario : Test
    {
        [Given]
        // ReSharper disable once UnusedMember.Local
        private void GivenThatIsIncorrectlyPrivate()
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