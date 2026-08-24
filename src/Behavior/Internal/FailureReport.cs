using System;
using System.Linq;
using Behavior.Internal.Exceptions;

namespace Behavior.Internal;

/// <summary>
/// Turns a step failure into what a reader should see. A failure arrives wrapped in an exception
/// naming the scenario and the step, both of which the surrounding Gherkin already shows, and
/// reflection leaves frames between the throw site and the step method. What's left after
/// taking both away is the cause and the line the reader wrote.
/// </summary>
static class FailureReport
{
    static readonly string[] Noise =
    {
        "System.Reflection.",
        "System.RuntimeMethodHandle",
        "System.Runtime.ExceptionServices.",
        "System.Runtime.CompilerServices.TaskAwaiter",
        // The runtime's generated invoke stub, named after the type it was emitted for, so it
        // shares no namespace with anything above.
        "InvokeStub_",
        // Named one by one rather than as the "Behavior." root: a consumer is free to put their
        // scenarios in a namespace of their own that starts the same way, and stripping their
        // frames would leave a failure with nothing to point at.
        "Behavior.Internal.",
        "Behavior.ScenarioBase",
        "Behavior.BehaviorTestFramework",
    };

    /// <summary>The exception to report, carrying the cleaned message and the filtered stack.</summary>
    public static Exception Of(Exception failure)
    {
        var cause = Cause(failure);
        var reason = Reason(cause);
        var stackTrace = Filtered(cause.StackTrace);

        return reason == cause.Message && stackTrace == cause.StackTrace
            ? cause
            : new Reported(cause, reason, stackTrace);
    }

    /// <summary>
    /// One line saying why, for the failing step's marker in the Gherkin. A wrapper's
    /// Message opens with an "Error in '&lt;scenario&gt;':" header, so its first line says
    /// nothing the reader can act on and the cause is below it.
    /// </summary>
    public static string OneLine(Exception failure)
    {
        var reason = Reason(Cause(failure));
        var newline = reason.IndexOfAny(new[] { '\r', '\n' });

        return newline < 0 ? reason : reason.Substring(0, newline);
    }

    /// <summary>
    /// Unwraps the step wrappers and stops. A step failure arrives wrapped in a GivenFailed,
    /// WhenFailed or ThenFailed whose whole message is "'&lt;step&gt;' failed" — nothing the
    /// Gherkin doesn't already show, so those are unwrapped. Every other ScenarioException *is*
    /// the diagnosis and holds the cause as its inner: WrongExceptionType knows which type was
    /// expected and which arrived, and ExpectedExceptionNotCaught knows Catch was never called.
    /// Unwrapping past those reports the exception the scenario asked for as though it were the
    /// failure, which reads as the test failing for the reason it was written to expect.
    /// A StepName is what separates the two, and only the step wrappers set one.
    /// </summary>
    public static Exception Cause(Exception exception)
    {
        while (exception is ScenarioException wrapper
               && !string.IsNullOrEmpty(wrapper.StepName)
               && wrapper.InnerException != null)
        {
            exception = wrapper.InnerException;
        }

        return exception;
    }

    static string Reason(Exception exception)
    {
        var reason = exception is ScenarioException scenario && !string.IsNullOrWhiteSpace(scenario.Reason)
            ? scenario.Reason
            : exception.Message;

        return string.IsNullOrWhiteSpace(reason) ? exception.GetType().Name : reason.Trim();
    }

    static string Filtered(string stackTrace)
    {
        if (string.IsNullOrEmpty(stackTrace))
        {
            return stackTrace;
        }

        var lines = stackTrace.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var kept = lines.Where(line => !IsNoise(line)).ToArray();

        // Nothing surviving means every frame was the framework or reflection, so there is no line the
        // reader wrote to point at — which is the truth for a compliance failure like "Catch was
        // never called", detected after the last step rather than inside one. A throw from a
        // library keeps its own frames, so this doesn't swallow those.
        return kept.Length == 0 ? null : string.Join(Environment.NewLine, kept);
    }

    static bool IsNoise(string line)
    {
        var frame = FrameOf(line);

        return frame != null && Noise.Any(n => frame.StartsWith(n, StringComparison.Ordinal));
    }

    /// <summary>
    /// A frame reads "&lt;at-word&gt; &lt;method&gt; in &lt;file&gt;:line &lt;n&gt;", and the
    /// at-word is localized. Everything after the first space is the part worth matching on.
    /// </summary>
    static string FrameOf(string line)
    {
        var trimmed = line.TrimStart();
        var space = trimmed.IndexOf(' ');

        return space < 0 ? null : trimmed.Substring(space + 1);
    }

    /// <summary>
    /// Message and StackTrace are the only things a reporter reads, and StackTrace is settable
    /// no other way. The original is deliberately not the InnerException: unwrapping is how the
    /// failure got here, and a caller that unwraps again would undo all of this.
    /// </summary>
    sealed class Reported : Exception
    {
        readonly Exception _original;
        readonly string _stackTrace;

        public Reported(Exception original, string message, string stackTrace)
            : base(message)
        {
            _original = original;
            _stackTrace = stackTrace;
        }

        public override string StackTrace => _stackTrace;

        public override string ToString() =>
            $"{_original.GetType()}: {Message}{Environment.NewLine}{_stackTrace}";
    }
}
