using System;
using System.Runtime.CompilerServices;
using Kekiri.Xunit.Infrastructure;
using Xunit;
using Xunit.v3;

namespace Kekiri.Xunit
{
    [XunitTestCaseDiscoverer(typeof(ScenarioDiscoverer))]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ScenarioAttribute : FactAttribute
    {
        public ScenarioAttribute(
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = -1)
            : base(sourceFilePath, sourceLineNumber)
        {
        }
    }
}
