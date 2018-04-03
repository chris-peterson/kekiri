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
            (test.Fixture as FeatureBase).Initialize();
        }

        public void AfterTest(ITest test)
        {
            (test.Fixture as FeatureBase).RunAsync().Wait();
        }

        public ActionTargets Targets => ActionTargets.Test | ActionTargets.Suite;
    }
}
