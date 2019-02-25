using Kekiri.Examples.WebApp;
using Kekiri.NUnit;
using NUnit.Framework;

namespace Kekiri.Examples.NUnit
{
    public class Context
    {
        public Context(IFoo foo)
        {
            Foo = foo;
        }

        public IFoo Foo { get; }
    }

    public class Context_injection : Scenarios<Context>
    {
        [ScenarioOutline]
        [Example(1)]
        [Example(2)]
        public void Service_overrides_work(int counter)
        {
            When(Running_scenario, counter);
            Then(It_should_use_correct_types, counter);
        }
    
        void Running_scenario(int counter) {
            // no-op
        }

        void It_should_use_correct_types(int counter) {
            Assert.IsInstanceOf<TestFoo>(Context.Foo);
        }
    }
}