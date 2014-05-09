using System;
using FluentAssertions;
using Kekiri.TestSupport.Scenarios.Exceptions;

namespace Kekiri.UnitTests.Exceptions
{
    [Scenario(Feature.TestExceptionHandling)]
    class When_test_throws_expected_exception : ScenarioTest
    {
        readonly When_test_throws_expected_exception_scenario _scenario = new When_test_throws_expected_exception_scenario();

        [When]
        public void When()
        {
            _scenario.SetupScenario();
            _scenario.It_should_throw_the_right_exception();
            _scenario.CleanupScenario();
        }

        [Then]
        public void It_should_provide_exception_to_the_test_for_validation()
        {
            _scenario.Exception.Should().BeOfType<ApplicationException>();
        }
    }
}