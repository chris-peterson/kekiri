using System;

namespace Kekiri.Impl.Exceptions
{
    class GivenFailed : ScenarioException
    {
        public GivenFailed(object test, string stepName, Exception innerException) :
            base(test, $"'{stepName}' failed", innerException)
        {
        }
    }
}