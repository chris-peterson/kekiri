using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Kekiri.NUnit
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ScenarioAttribute : TestAttribute, ITestAction
    {
        public void BeforeTest(ITest test)
        {
            var scenario = test.Fixture as ScenarioBase;
            if (scenario != null)
            {
                scenario.Initialize();
            }
        }

        public void AfterTest(ITest test)
        {
            var scenario = test.Fixture as ScenarioBase;
            if (scenario != null)
            {
                scenario.RunAsync().Wait();
            }
        }

        public ActionTargets Targets => ActionTargets.Test | ActionTargets.Suite;
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ScenarioOutlineAttribute : ScenarioAttribute
    {
    }
}
