using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Kekiri.TestRunner.Xunit
{
    //[TestFrameworkDiscoverer("Kekiri.TestRunner.Xunit.Infrastructure.ScenarioFrameworkDiscoverer", "Kekiri.TestRunner.Xunit")]
    public class Scenario : ScenarioBase
    {
        [Fact]
        public override void Run()
        {
            base.Run();
        }
    }

    //[TestFrameworkDiscoverer("Kekiri.TestRunner.Xunit.Infrastructure.ScenarioFrameworkDiscoverer", "Kekiri.TestRunner.Xunit")]
    public class Scenario<TContext> : ScenarioBase<TContext>
    {
        [Fact]
        public override void Run()
        {
            base.Run();
        }
    }

    /*
    public class ScenarioInvoker : XunitTestInvoker
    {
        public ScenarioInvoker(ITest test,
            IMessageBus messageBus,
            Type testClass, object[] constructorArguments,
            MethodInfo testMethod,
            object[] testMethodArguments,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource) : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, beforeAfterAttributes, aggregator, cancellationTokenSource)
        {
        }

        protected override object CreateTestClass()
        {
            var constructorsWithExamples = TestClass
                .GetTypeInfo()
                .GetConstructors()
                .Where(c => c.GetCustomAttributes<ExampleAttribute>().Any())
                .ToArray();

            if (!constructorsWithExamples.Any())
            {
                return base.CreateTestClass();
            }

            if (constructorsWithExamples.Length > 1)
            {
                throw new NotSupportedException($"Error with scenario {TestClass.FullName}. A scenario can only have one constructor.");
            }

            var ctor = constructorsWithExamples.Single();
            var examples = ctor.GetCustomAttributes<ExampleAttribute>();


        }


    }*/
}