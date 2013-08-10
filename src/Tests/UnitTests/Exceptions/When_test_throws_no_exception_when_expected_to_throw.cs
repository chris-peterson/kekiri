using Kekiri.Exceptions;
using Kekiri.TestSupport.Scenarios.Exceptions;

namespace Kekiri.UnitTests.Exceptions
{
    class When_test_throws_no_exception_when_expected_to_throw : TestExceptionScenarioTest
    {
        [When, Throws]
        public void When()
        {
            var scenario = new When_test_throws_no_exception_when_expected_to_throw_scenario();
            scenario.SetupScenario();
            scenario.CleanupScenario();
        }

        [Then]
        public void It_should_have_expected_exception_in_exception_details()
        {
            Catch<NoExceptionThrown>();
        }
    }
}