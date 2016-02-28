using System;

namespace Kekiri.Impl.Exceptions
{
    class ScenarioException : Exception
    {
        public object Test { get; private set; }

        public ScenarioException(object test, string message) :
            this(test, message, null)
        {
        }

        public ScenarioException(Type type, string message) :
            this(type, message, null)
        {
        }

        public ScenarioException(object test, string message, Exception innerException) :
            this(test.GetType(), message, innerException)
        {
            Test = test;
        }

        public ScenarioException(Type type, string message, Exception innerException) :
            base($"Error in '{type.Name}':\r\n{message}", innerException)
        {
        }
    }
}