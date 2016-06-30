using System;

namespace Kekiri.Impl.Exceptions
{
    class ExpectedExceptionNotCaught : ScenarioException
    {
        public ExpectedExceptionNotCaught(ScenarioBase scenario, Exception exception) :
            base(scenario, $"Exception was expected but Catch<{exception.GetType()}>() was not called", exception)
        {
        }
    }
}