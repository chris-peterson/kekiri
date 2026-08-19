using System;
using System.Runtime.CompilerServices;
using Kekiri.Xunit.Infrastructure;
using Xunit;
using Xunit.v3;

namespace Kekiri.Xunit
{
    [XunitTestCaseDiscoverer(typeof(ScenarioOutlineDiscoverer))]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ScenarioOutlineAttribute : TheoryAttribute
    {
        public ScenarioOutlineAttribute(
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = -1)
            : base(sourceFilePath, sourceLineNumber)
        {
        }
    }
}
