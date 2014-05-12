using System;

namespace Kekiri.Exceptions
{
    internal class WhenFailed : ScenarioTestException
    {
        public WhenFailed(object test, string stepName, Exception innerException) :
            base(test, string.Format("'{0}' threw an exception.  If this is expected behavior use [Throws] attribute and add test(s) with Catch<>", stepName), innerException)
        {
        }
    }
}
