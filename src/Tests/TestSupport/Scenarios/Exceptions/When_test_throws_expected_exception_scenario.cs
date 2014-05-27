using System;

namespace Kekiri.TestSupport.Scenarios.Exceptions
{
    public class When_test_throws_expected_exception_scenario : Test
    {
        [When, Throws]
        public void When()
        {
            throw new ApplicationException();
        }

        [Then]
        public void It_should_throw_the_right_exception()
        {
            Exception = Catch<ApplicationException>();
        }

        public Exception Exception { get; private set; }
    }
}