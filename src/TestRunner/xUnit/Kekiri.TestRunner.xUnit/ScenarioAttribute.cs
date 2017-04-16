using System;
using Xunit;
using Xunit.Sdk;

namespace Kekiri.TestRunner.xUnit
{
    [XunitTestCaseDiscoverer("Kekiri.TestRunner.xUnit.Infrastructure.ScenarioDiscoverer", "Kekiri.TestRunner.xUnit")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ScenarioAttribute : FactAttribute
    {

    }
}