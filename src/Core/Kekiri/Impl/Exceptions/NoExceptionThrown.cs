using System;

namespace Kekiri.Impl.Exceptions
{
    class NoExceptionThrown : ScenarioException
    {
        public NoExceptionThrown(
            ScenarioBase scenario) :
            base(scenario, "No exception was thrown, but was expected", null)
        {
        }

        public NoExceptionThrown(
            ScenarioBase scenario,
            Type expectedExceptionType) :
            base(scenario, $"No exception was thrown, but was looking for '{expectedExceptionType}'", null)
        {
        }
    }
}