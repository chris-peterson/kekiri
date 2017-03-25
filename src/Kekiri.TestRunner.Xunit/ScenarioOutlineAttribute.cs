using System;
using Xunit;
using Xunit.Sdk;

namespace Kekiri.TestRunner.Xunit
{
    [XunitTestCaseDiscoverer("Kekiri.TestRunner.Xunit.Infrastructure.ScenarioOutlineDiscoverer", "Kekiri.TestRunner.Xunit")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ScenarioOutlineAttribute : FactAttribute
    {
    }
}