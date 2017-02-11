using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Kekiri.TestRunner.Xunit.Infrastructure
{
    public class ScenarioCollectionRunner : XunitTestCollectionRunner
    {
        private IMessageSink DiagnosticMessageSink { get; }

        public ScenarioCollectionRunner(ITestCollection testCollection, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ITestCaseOrderer testCaseOrderer, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource) : base(testCollection, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource)
        {
            DiagnosticMessageSink = diagnosticMessageSink;
        }

        protected override async Task<RunSummary> RunTestClassAsync(ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<IXunitTestCase> testCases)
        {
            if (typeof(ExampleTestCase).GetTypeInfo().IsAssignableFrom(testCases.First().GetType()))
            {
                var aggregateSummary = new RunSummary();
                foreach (ExampleTestCase testCase in testCases)
                {
                    var runner = new ScenarioClassRunner(testClass, @class, testCase, DiagnosticMessageSink, MessageBus, TestCaseOrderer, Aggregator, CancellationTokenSource, CollectionFixtureMappings);

                    var summary = await runner.RunAsync();

                    aggregateSummary.Aggregate(summary);
                }

                return aggregateSummary;
            }

            return await base.RunTestClassAsync(testClass, @class, testCases);
        }
    }
}