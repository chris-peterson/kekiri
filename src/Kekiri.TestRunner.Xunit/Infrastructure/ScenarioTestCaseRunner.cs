using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Kekiri.TestRunner.Xunit.Infrastructure
{
    class ScenarioTestCaseRunner : XunitTestCaseRunner
    {
        public ScenarioTestCaseRunner(IXunitTestCase testCase, string displayName, string skipReason, object[] constructorArguments, object[] testMethodArguments, IMessageBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource) : base(testCase, displayName, skipReason, constructorArguments, testMethodArguments, messageBus, aggregator, cancellationTokenSource)
        {
        }

        protected override Task<RunSummary> RunTestAsync()
        {
            return new ScenarioTestRunner((ITest) new XunitTest(this.TestCase, this.DisplayName), this.MessageBus, this.TestClass, this.ConstructorArguments, this.TestMethod, this.TestMethodArguments, this.SkipReason, BeforeAfterAttributes, new ExceptionAggregator(this.Aggregator), this.CancellationTokenSource).RunAsync();
        }
    }
}