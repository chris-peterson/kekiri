namespace Kekiri.TestSupport.Scenarios
{
    public class When_overriding_fixture_setup_methods_scenario : SuppressedOutputScenarioTest
    {
        public int SetupScenairoCalledCount { get; private set; }
        public int CleanupScenarioCalledCount { get; private set; }

        public override void SetupScenario()
        {
            base.SetupScenario();
            SetupScenairoCalledCount++;
        }

        public override void CleanupScenario()
        {
            base.CleanupScenario();
            CleanupScenarioCalledCount++;
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