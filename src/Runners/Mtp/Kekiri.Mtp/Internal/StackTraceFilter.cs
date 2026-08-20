using System;
using System.Linq;

namespace Kekiri.Mtp.Internal
{
    /// <summary>
    /// Drops the frames between the step method and the reflection call that reached it. A reader
    /// looking at a failed Then wants the line they wrote and nothing under it; xUnit and NUnit
    /// both ship a filter of this shape for the same reason.
    /// </summary>
    static class StackTraceFilter
    {
        static readonly string[] Noise =
        {
            "System.Reflection.",
            "System.RuntimeMethodHandle",
            "System.Runtime.ExceptionServices.",
            "System.Runtime.CompilerServices.TaskAwaiter",
            "Kekiri.Impl.",
            "Kekiri.ScenarioBase",
            "Kekiri.Mtp.Internal.",
            "Kekiri.Mtp.KekiriTestFramework",
            "Kekiri.Mtp.Scenarios",
        };

        public static Exception Clean(Exception exception)
        {
            var stackTrace = exception.StackTrace;

            if (string.IsNullOrEmpty(stackTrace))
            {
                return exception;
            }

            var lines = stackTrace.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var kept = lines.Where(line => !IsNoise(line)).ToArray();

            // Nothing dropped needs no wrapper; everything dropped means the throw site was inside
            // a library, and a trace of nothing is worse than a noisy one.
            if (kept.Length == lines.Length || kept.Length == 0)
            {
                return exception;
            }

            return new FilteredStackTrace(exception, string.Join(Environment.NewLine, kept));
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
        /// StackTrace is virtual, which is the only way to hand a reporter a trace it didn't
        /// capture. The original is deliberately not the InnerException: unwrapping is how the
        /// failure reached here, and a caller that unwraps again would undo the filtering.
        /// </summary>
        sealed class FilteredStackTrace : Exception
        {
            readonly Exception _original;
            readonly string _stackTrace;

            public FilteredStackTrace(Exception original, string stackTrace)
                : base(original.Message)
            {
                _original = original;
                _stackTrace = stackTrace;
            }

            public override string StackTrace => _stackTrace;

            public override string ToString() =>
                $"{_original.GetType()}: {Message}{Environment.NewLine}{_stackTrace}";
        }
    }
}
