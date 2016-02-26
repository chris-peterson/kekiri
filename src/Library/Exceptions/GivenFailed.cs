using System;

namespace Kekiri.Exceptions
{
    class GivenFailed : ScenarioException
    {
        public GivenFailed(object test, string stepName, Exception innerException) :
            base(test, $"'{stepName}' failed", innerException)
        {
        }
    }
}