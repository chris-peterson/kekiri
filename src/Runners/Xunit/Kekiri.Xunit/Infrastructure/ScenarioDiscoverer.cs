using System;
using Xunit.Sdk;
using Xunit.v3;

namespace Kekiri.Xunit.Infrastructure
{
    public class ScenarioDiscoverer : FactDiscoverer
    {
        protected override IXunitTestCase CreateTestCase(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            IXunitTestMethod testMethod,
            IFactAttribute factAttribute)
        {

            var details = TestIntrospectionHelper.GetTestCaseDetails(
                discoveryOptions,
                testMethod,
                factAttribute,
                baseDisplayName: ScenarioTestCaseFactory.DisplayName(testMethod));

            return new ScenarioTestCase(
                details.ResolvedTestMethod,
                details.TestCaseDisplayName,
                details.UniqueID,
                details.Explicit,
                details.SkipExceptions,
                details.SkipReason,
                details.SkipType,
                details.SkipUnless,
                details.SkipWhen,
                ScenarioTestCaseFactory.ToReadWrite(testMethod.Traits),
                sourceFilePath: details.SourceFilePath,
                sourceLineNumber: details.SourceLineNumber,
                timeout: details.Timeout);
        }
    }
}
