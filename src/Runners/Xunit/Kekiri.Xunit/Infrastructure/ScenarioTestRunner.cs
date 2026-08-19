using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace Kekiri.Xunit.Infrastructure
{
    class ScenarioTestRunner : XunitTestRunnerBase<XunitTestRunnerContext, IXunitTest>
    {
        public static ScenarioTestRunner Instance { get; } = new ScenarioTestRunner();

        public async ValueTask<RunSummary> Run(
            IXunitTest test,
            IMessageBus messageBus,
            object[] constructorArguments,
            ExplicitOption explicitOption,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource,
            ParallelMode parallelMode,
            ExecutionScheduler scheduler,
            IReadOnlyCollection<IBeforeAfterTestAttribute> beforeAfterAttributes,
            FixtureMappingManager caseFixtureMappings)
        {
            await using var ctxt = new XunitTestRunnerContext(
                test, explicitOption, messageBus, aggregator, cancellationTokenSource, parallelMode,
                scheduler, beforeAfterAttributes, constructorArguments, caseFixtureMappings);

            await ctxt.InitializeAsync();

            return await Run(ctxt);
        }

        /// <summary>
        /// A scenario test method only *records* its Given/When/Then chain; running that chain is a
        /// second step, and its failures have to surface as test failures, so it happens here rather
        /// than in a Dispose or an After hook.
        /// </summary>
        protected override async ValueTask<TimeSpan> InvokeTest(
            XunitTestRunnerContext ctxt,
            object testClassInstance)
        {
            var elapsed = await base.InvokeTest(ctxt, testClassInstance);

            if (testClassInstance is ScenarioBase scenario)
            {
                elapsed += await ExecutionTimer.MeasureAsync(
                    () => ctxt.Aggregator.RunAsync(() => new ValueTask(scenario.RunAsync())));
            }

            return elapsed;
        }
    }
}
