using System;
using FluentAssertions;
using Kekiri.Exceptions;
using Kekiri.TestSupport.Scenarios.Exceptions;

namespace Kekiri.UnitTests.Exceptions
{
    [Scenario(Feature.TestExceptionHandling)]
    class When_test_throws_wrong_exception_type : ScenarioTest
    {
        [When, Throws]
        public void When()
        {
            var scenario = new When_test_throws_wrong_exception_type_scenario();
            scenario.SetupScenario();
            scenario.AskForWrongException();
        }

        [Then]
        public void It_should_throw_wrong_exception_type()
        {
            Catch<WrongExceptionType>();
        }

        [Then]
        public void Exception_details_contain_expected_exception()
        {
            Catch<WrongExceptionType>().ExpectedExceptionType.Should().Be<ApplicationException>();
        }

        [Then]
        public void Exception_details_contain_actual_exception()
        {
            Catch<WrongExceptionType>().ActualExceptionType.Should().Be<ArgumentException>();
        }
    }
}