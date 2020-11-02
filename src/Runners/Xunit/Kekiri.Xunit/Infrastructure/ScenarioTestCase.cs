using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Kekiri.Xunit.Infrastructure
{
    class ScenarioTestCase : XunitTestCase
    {
        #pragma warning disable 0618
        //required for serialization by xunit framework
        public ScenarioTestCase() {}
        #pragma warning restore 0618

        public ScenarioTestCase(IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay, ITestMethod testMethod, object[] testMethodArguments = null) : base(diagnosticMessageSink, defaultMethodDisplay, TestMethodDisplayOptions.ReplaceUnderscoreWithSpace, testMethod, testMethodArguments)
        {
        }

        public override Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments,
            ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        {
            return new ScenarioTestCaseRunner((IXunitTestCase) this, this.DisplayName, this.SkipReason, constructorArguments, this.TestMethodArguments, messageBus, aggregator, cancellationTokenSource).RunAsync();
        }
    }
}