using System;
using Xunit;
using Xunit.Sdk;

namespace Kekiri.TestRunner.Xunit
{
    [XunitTestCaseDiscoverer("Kekiri.TestRunner.Xunit.Infrastructure.ScenarioDiscoverer", "Kekiri.TestRunner.Xunit")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ScenarioAttribute : FactAttribute
    {

    }
}