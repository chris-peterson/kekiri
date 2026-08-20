using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace Kekiri.Xunit.Infrastructure
{
    public class ScenarioTheoryTestCase : XunitDelayEnumeratedTheoryTestCase, ISelfExecutingXunitTestCase
    {
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public ScenarioTheoryTestCase()
        {
        }

        public ScenarioTheoryTestCase(
            IXunitTestMethod testMethod,
            string testCaseDisplayName,
            string uniqueID,
            bool @explicit,
            bool skipTestWithoutData,
            Type[] skipExceptions = null,
            string skipReason = null,
            Type skipType = null,
            string skipUnless = null,
            string skipWhen = null,
            Dictionary<string, HashSet<string>> traits = null,
            string sourceFilePath = null,
            int? sourceLineNumber = null,
            int? timeout = null)
            : base(testMethod, testCaseDisplayName, uniqueID, @explicit, skipTestWithoutData, skipExceptions,
                skipReason, skipType, skipUnless, skipWhen, traits, sourceFilePath, sourceLineNumber, timeout)
        {
        }

        public ValueTask<RunSummary> Run(
            ExplicitOption explicitOption,
            IMessageBus messageBus,
            object[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource,
            ParallelMode parallelMode,
            ExecutionScheduler scheduler,
            FixtureMappingManager methodFixtureMappings) =>
                ScenarioTestCaseFactory.Run(this, explicitOption, messageBus, constructorArguments, aggregator,
                    cancellationTokenSource, parallelMode, scheduler, methodFixtureMappings);
    }
}
