using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace Kekiri.Xunit.Infrastructure
{
    class ScenarioTestCaseRunnerContext : XunitTestCaseRunnerBaseContext<IXunitTestCase, IXunitTest>
    {
        public ScenarioTestCaseRunnerContext(
            IXunitTestCase testCase,
            IReadOnlyCollection<IXunitTest> tests,
            ExplicitOption explicitOption,
            IMessageBus messageBus,
            ExceptionAggregator aggregator,
            string displayName,
            string skipReason,
            CancellationTokenSource cancellationTokenSource,
            ParallelMode parallelMode,
            ExecutionScheduler scheduler,
            object[] constructorArguments,
            FixtureMappingManager methodFixtureMappings)
            : base(testCase, tests, explicitOption, messageBus, aggregator, displayName, skipReason,
                cancellationTokenSource, parallelMode, scheduler, constructorArguments, methodFixtureMappings)
        {
        }

        public override ValueTask<RunSummary> RunTest(IXunitTest test) =>
            ScenarioTestRunner.Instance.Run(
                test,
                MessageBus,
                ConstructorArguments,
                ExplicitOption,
                Aggregator.Clone(),
                CancellationTokenSource,
                ParallelMode,
                Scheduler,
                BeforeAfterTestAttributes,
                CaseFixtureMappings);
    }

    class ScenarioTestCaseRunner :
        XunitTestCaseRunnerBase<ScenarioTestCaseRunnerContext, IXunitTestCase, IXunitTest>
    {
        public static ScenarioTestCaseRunner Instance { get; } = new ScenarioTestCaseRunner();

        public async ValueTask<RunSummary> Run(
            IXunitTestCase testCase,
            IReadOnlyCollection<IXunitTest> tests,
            IMessageBus messageBus,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource,
            ParallelMode parallelMode,
            ExecutionScheduler scheduler,
            string displayName,
            string skipReason,
            ExplicitOption explicitOption,
            object[] constructorArguments,
            FixtureMappingManager methodFixtureMappings)
        {
            await using var ctxt = new ScenarioTestCaseRunnerContext(
                testCase, tests, explicitOption, messageBus, aggregator, displayName, skipReason,
                cancellationTokenSource, parallelMode, scheduler, constructorArguments, methodFixtureMappings);

            await ctxt.InitializeAsync();

            return await Run(ctxt);
        }
    }
}
