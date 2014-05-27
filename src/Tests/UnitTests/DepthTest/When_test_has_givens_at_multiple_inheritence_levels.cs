using FluentAssertions;
using Kekiri.TestSupport.Scenarios.DepthTest;

namespace Kekiri.UnitTests.DepthTest
{
    [Scenario(Feature.Subclassing)]
    class When_test_has_givens_at_multiple_inheritence_levels : Test
    {
        readonly When_scenario_test_has_derived_depth2 _test = new When_scenario_test_has_derived_depth2();

        [When]
        public void When()
        {
            _test.SetupScenario();
        }

        [Then]
        public void It_should_have_correct_number_of_invocations()
        {
            _test.Levels.Count.Should().Be(3);
        }

        [Then]
        public void It_should_call_base_first()
        {
            _test.Levels[0].Should().Be(ScenarioDepthTestLevel.Base);
        }

        [Then]
        public void It_should_call_depth1_second()
        {
            _test.Levels[1].Should().Be(ScenarioDepthTestLevel.Depth1);
        }

        [Then]
        public void It_should_call_depth2_third()
        {
            _test.Levels[2].Should().Be(ScenarioDepthTestLevel.Depth2);
        }
    }
}