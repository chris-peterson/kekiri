using System;

namespace Kekiri.TestSupport.Scenarios.Exceptions
{
    public class When_test_throws_wrong_exception_type_scenario : ScenarioTest
    {
        [When, Throws]
        public void When()
        {
            throw new ArgumentException();
        }

        [Then]
        public void Then()
        {
        }

        public void AskForWrongException()
        {
            Catch<ApplicationException>();
        }
    }
}