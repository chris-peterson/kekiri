using System;
using Kekiri.Xunit;

namespace Kekiri.Examples.Xunit
{
    public class Context
    {
        public Context(ExampleService service)
        {
            Service = service;
        }

        public ExampleService Service { get; }
    }

    public class ExampleService
    {        
    }

    public class Context_injection : ExampleScenariosTyped<Context>
    {
        [ScenarioOutline]
        [Example(1)]
        [Example(2)]
        public void Each_example_in_a_scenario_outline_runs_in_separate_lifetime_scope(int counter)
        {
            When(Running_scenario, counter);
            Then(It_should_use_different_objects);
        }
    
        void Running_scenario(int counter) {
            // no-op
        }

        void It_should_use_different_objects() {
            Console.WriteLine($"This:\n\t{this.GetHashCode()}");
            Console.WriteLine($"Context:\n\t{Context.GetHashCode()}");
            Console.WriteLine($"Resolved Service:\n\t{Context.Service.GetHashCode()}");
        }
    }
}