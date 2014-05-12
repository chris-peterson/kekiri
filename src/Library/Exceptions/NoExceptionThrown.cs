using System;

namespace Kekiri.Exceptions
{
    internal class NoExceptionThrown : ScenarioTestException
    {
        public NoExceptionThrown(
            object test) :
            base(test, "No exception was thrown, but was expected as indicated by [Throws]", null)
        {
        }

        public NoExceptionThrown(
            object test,
            Type expectedExceptionType) :
            base(test, string.Format("No exception was thrown, but was looking for '{0}'", expectedExceptionType), null)
        {
        }
    }
}