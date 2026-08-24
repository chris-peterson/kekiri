using System;

namespace Behavior.Internal.Exceptions;

class ScenarioException : Exception
{
    public ScenarioBase Scenario { get; private set; }

    /// <summary>
    /// The step whose failure caused this, when there was one. A runner can point at the step
    /// rather than leaving a reader to find it inside the message text.
    /// </summary>
    public string StepName { get; private set; }

    /// <summary>
    /// The message without the "Error in '&lt;scenario&gt;':" header Message carries. A runner
    /// that already names the scenario on screen wants the cause on its own, and the header is
    /// the first line, so taking the first line of Message yields the header and nothing else.
    /// </summary>
    public string Reason { get; private set; }

    public ScenarioException(ScenarioBase scenario, string message) :
        this(scenario, message, null)
    {
    }

    public ScenarioException(ScenarioBase scenario, string message, Exception innerException) :
        this(scenario.GetType(), message, innerException)
    {
        Scenario = scenario;
    }

    protected ScenarioException(ScenarioBase scenario, string stepName, string message, Exception innerException) :
        this(scenario, message, innerException)
    {
        StepName = stepName;
    }


    public ScenarioException(Type type, string message) :
        this(type, message, null)
    {
    }

    public ScenarioException(Type type, string message, Exception innerException) :
        base($"Error in '{type.Name}':\r\n{message}", innerException)
    {
        Reason = message;
    }
}
