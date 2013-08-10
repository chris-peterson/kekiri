using System;

namespace Kekiri.Exceptions
{
    public class NoExceptionThrown : ScenarioTestException
    {
        public NoExceptionThrown(
            ScenarioTest test) :
            base(test, "No exception was thrown, but was expected as indicated by [Throws]", null)
        {
        }

        public NoExceptionThrown(
            ScenarioTest test,
            Type expectedExceptionType) :
            base(test, string.Format("No exception was thrown, but was looking for '{0}'", expectedExceptionType), null)
        {
        }
    }
}