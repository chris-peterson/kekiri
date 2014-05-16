using System;

namespace Kekiri.Exceptions
{
    internal class ExpectedExceptionNotCaught : ScenarioTestException
    {
        public ExpectedExceptionNotCaught(object test, Exception exception) :
            base(test, string.Format("Exception was expected but Catch<{0}>() was not called", exception.GetType()), exception)
        {
        }
    }
}