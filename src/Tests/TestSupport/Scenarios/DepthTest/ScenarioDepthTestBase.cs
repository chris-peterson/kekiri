using System.Collections.Generic;

namespace Kekiri.TestSupport.Scenarios.DepthTest
{
    public abstract class ScenarioDepthTestBase : ScenarioTest
    {
        private readonly List<ScenarioDepthTestLevel> _levels = new List<ScenarioDepthTestLevel>();

        public List<ScenarioDepthTestLevel> Levels
        {
            get { return _levels; }
        }

        [Given]
        public void Given_base()
        {
            _levels.Add(ScenarioDepthTestLevel.Base);
        }

        [When]
        public void When()
        {
        }

        [Then]
        public void It_should_do_nothing()
        {
        }
    }
}