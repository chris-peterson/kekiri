using System;

namespace Kekiri.Impl.Exceptions
{
    class NoExceptionThrown : ScenarioException
    {
        public NoExceptionThrown(
            object test) :
            base(test, "No exception was thrown, but was expected", null)
        {
        }

        public NoExceptionThrown(
            object test,
            Type expectedExceptionType) :
            base(test, $"No exception was thrown, but was looking for '{expectedExceptionType}'", null)
        {
        }
    }
}