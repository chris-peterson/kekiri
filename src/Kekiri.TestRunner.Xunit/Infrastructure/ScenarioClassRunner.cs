using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Kekiri.TestRunner.Xunit.Infrastructure
{
    public class ScenarioClassRunner : XunitTestClassRunner
    {
        private ExampleTestCase TestCase { get; }

        public ScenarioClassRunner(ITestClass testClass,
            IReflectionTypeInfo @class,
            ExampleTestCase testCase,
            IMessageSink diagnosticMessageSink,
            IMessageBus messageBus,
            ITestCaseOrderer testCaseOrderer,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource,
            IDictionary<Type, object> collectionFixtureMappings)
            : base(testClass,
                @class,
                new IXunitTestCase[] { testCase },
                diagnosticMessageSink,
                messageBus,
                testCaseOrderer,
                aggregator,
                cancellationTokenSource,
                collectionFixtureMappings)
        {
            TestCase = testCase;
        }

        protected override bool TryGetConstructorArgument(ConstructorInfo constructor, int index, ParameterInfo parameter, out object argumentValue)
        {
            argumentValue = TestCase.Example.Values[index];
            return true;
        }
    }
}