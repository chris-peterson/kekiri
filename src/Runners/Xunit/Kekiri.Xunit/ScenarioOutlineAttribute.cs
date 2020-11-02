using System;
using Xunit;
using Xunit.Sdk;

namespace Kekiri.Xunit
{
    [XunitTestCaseDiscoverer("Kekiri.Xunit.Infrastructure.ScenarioOutlineDiscoverer", "Kekiri.Xunit")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ScenarioOutlineAttribute : TheoryAttribute
    {
    }
}