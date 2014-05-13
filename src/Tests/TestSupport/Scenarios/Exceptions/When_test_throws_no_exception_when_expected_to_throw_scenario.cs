namespace Kekiri.TestSupport.Scenarios.Exceptions
{
    public class When_test_throws_no_exception_when_expected_to_throw_scenario : Test
    {
        [When, Throws]
        public void When()
        {
        }

        [Then]
        public void Then()
        {
        }
    }
}