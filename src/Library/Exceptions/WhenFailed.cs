using System;
using System.Reflection;

namespace Kekiri.Exceptions
{
    public class WhenFailed : ScenarioTestException
    {
        public WhenFailed(ScenarioTest test, string stepName, Exception innerException) :
            base(test, string.Format("'{0}' threw an exception.  If this is expected behavior use [Throws] attribute and add test(s) with Catch<>", stepName), innerException)
        {
        }
    }
}
