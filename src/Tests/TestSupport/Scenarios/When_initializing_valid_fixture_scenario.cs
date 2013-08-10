namespace Kekiri.TestSupport.Scenarios
{
    public class When_initializing_valid_fixture_scenario : SuppressedOutputScenarioTest
    {
        [Given]
        public void Given()
        {
            GivenRunCount++;
        }

        [When]
        public void When()
        {
            WhenRunCount++;
        }

        [Then]
        public void Then()
        {
            ThenRunCount++;
        }

        public int GivenRunCount { get; set; }
        public int WhenRunCount { get; set; }
        public int ThenRunCount { get; set; }
    }
}