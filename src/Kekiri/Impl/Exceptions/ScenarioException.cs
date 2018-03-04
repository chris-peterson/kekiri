using System;

namespace Kekiri.Impl.Exceptions
{
    class ScenarioException : Exception
    {
        public ScenarioBase Scenario { get; private set; }

        public ScenarioException(ScenarioBase scenario, string message) :
            this(scenario, message, null)
        {
        }

        public ScenarioException(ScenarioBase scenario, string message, Exception innerException) :
            this(scenario.GetType(), message, innerException)
        {
            Scenario = scenario;
        }


        public ScenarioException(Type type, string message) :
            this(type, message, null)
        {
        }

        public ScenarioException(Type type, string message, Exception innerException) :
            base($"Error in '{type.Name}':\r\n{message}", innerException)
        {
        }
    }
}