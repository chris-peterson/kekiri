using System;

namespace Kekiri.Exceptions
{
    public class ScenarioTestException : Exception
    {
        public object Test { get; private set; }

        public ScenarioTestException(object test, string message) :
            this(test, message, null)
        {
        }

        public ScenarioTestException(Type scenarioType, string message) :
            this(scenarioType, message, null)
        {
        }

        public ScenarioTestException(object test, string message, Exception innerException) :
            this(test.GetType(), message, innerException)
        {
            Test = test;
        }

        public ScenarioTestException(Type scenarioType, string message, Exception innerException) :
            base(string.Format("Error in '{0}':\r\n{1}", scenarioType.Name, message), innerException)
        {
        }
    }
}