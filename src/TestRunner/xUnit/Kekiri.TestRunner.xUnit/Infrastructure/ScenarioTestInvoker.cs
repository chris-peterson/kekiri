using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Kekiri.TestRunner.xUnit.Infrastructure
{
    class ScenarioTestInvoker : XunitTestInvoker
    {
        public ScenarioTestInvoker(ITest test, IMessageBus messageBus, Type testClass, object[] constructorArguments, MethodInfo testMethod, object[] testMethodArguments, IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource) : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, beforeAfterAttributes, aggregator, cancellationTokenSource)
        {
        }

        protected override object CallTestMethod(object testClassInstance)
        {
            base.CallTestMethod(testClassInstance);

            if (testClassInstance is ScenarioBase scenarioInstance)
            {
                scenarioInstance.Run();
            }

            return null;
        }
    }
}