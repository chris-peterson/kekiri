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
        }

        public void AfterTest(ITest test)
        {
            var scenario = test.Fixture as ScenarioBase;
            if (scenario != null)
            {
                try
                {
                    scenario.RunAsync().Wait();
                }
                finally
                {
                    // NUnit reuses one fixture instance for every case in the class, so a
                    // failed run still has to hand the next case an empty scenario.  This
                    // runs after RunAsync rather than in BeforeTest so it can't discard a
                    // container the fixture has already resolved from.
                    scenario.Initialize();
                }
            }
        }

        public ActionTargets Targets => ActionTargets.Test | ActionTargets.Suite;
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ScenarioOutlineAttribute : ScenarioAttribute
    {
    }
}
