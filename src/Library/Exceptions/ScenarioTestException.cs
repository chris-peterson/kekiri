using System;

namespace Kekiri.Exceptions
{
    public class ScenarioTestException : Exception
    {
        public ScenarioTest Test { get; private set; }

        public ScenarioTestException(ScenarioTest test, string message) :
            this(test, message, null)
        {
        }

        public ScenarioTestException(ScenarioTest test, string message, Exception innerException) :
            base(string.Format("Error in '{0}':\r\n{1}", test.GetType().Name, message), innerException)
        {
            Test = test;
        }
    }
}