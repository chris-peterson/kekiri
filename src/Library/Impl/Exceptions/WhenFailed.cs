using System;

namespace Kekiri.Impl.Exceptions
{
    class WhenFailed : ScenarioException
    {
        public WhenFailed(object test, string stepName, Exception innerException) :
            base(test, $"'{stepName}' threw an exception.  If this is expected behavior use .Throws() and add a step that uses Catch<>", innerException)
        {
        }
    }
}
