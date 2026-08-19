using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

namespace Kekiri.Xunit.Infrastructure
{
    public class ScenarioOutlineDiscoverer : TheoryDiscoverer
    {
        protected override ValueTask<IReadOnlyCollection<IXunitTestCase>> CreateTestCasesForDataRow(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            IXunitTestMethod testMethod,
            ITheoryAttribute theoryAttribute,
            ITheoryDataRow dataRow,
            object[] testMethodArguments,
            string index)
        {
            ScenarioTestCaseFactory.GuardScenarioClass(testMethod, nameof(ScenarioOutlineAttribute));

            var details = TestIntrospectionHelper.GetTestCaseDetailsForTheoryDataRow(
                discoveryOptions, testMethod, theoryAttribute, dataRow, testMethodArguments, index);

            IReadOnlyCollection<IXunitTestCase> testCases = new IXunitTestCase[]
            {
                new ScenarioTestCase(
                    details.ResolvedTestMethod,
                    details.TestCaseDisplayName,
                    details.UniqueID,
                    details.Explicit,
                    dataRow.Label,
                    dataRow.DisableParallelization ?? false,
                    details.SkipExceptions,
                    details.SkipReason,
                    details.SkipType,
                    details.SkipUnless,
                    details.SkipWhen,
                    TestIntrospectionHelper.GetTraits(testMethod, dataRow),
                    testMethodArguments,
                    sourceFilePath: details.SourceFilePath,
                    sourceLineNumber: details.SourceLineNumber,
                    timeout: details.Timeout)
            };

            return new ValueTask<IReadOnlyCollection<IXunitTestCase>>(testCases);
        }

        // Reached when the theory data can't be pre-enumerated (non-serializable data, or
        // preEnumerateTheories turned off). The data rows are resolved at run time instead.
        protected override ValueTask<IReadOnlyCollection<IXunitTestCase>> CreateTestCasesForTheory(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            IXunitTestMethod testMethod,
            ITheoryAttribute theoryAttribute)
        {
            ScenarioTestCaseFactory.GuardScenarioClass(testMethod, nameof(ScenarioOutlineAttribute));

            var details = TestIntrospectionHelper.GetTestCaseDetails(
                discoveryOptions,
                testMethod,
                theoryAttribute,
                baseDisplayName: ScenarioTestCaseFactory.DisplayName(testMethod));

            IReadOnlyCollection<IXunitTestCase> testCases = new IXunitTestCase[]
            {
                new ScenarioTheoryTestCase(
                    details.ResolvedTestMethod,
                    details.TestCaseDisplayName,
                    details.UniqueID,
                    details.Explicit,
                    theoryAttribute.SkipTestWithoutData,
                    details.SkipExceptions,
                    details.SkipReason,
                    details.SkipType,
                    details.SkipUnless,
                    details.SkipWhen,
                    ScenarioTestCaseFactory.ToReadWrite(testMethod.Traits),
                    sourceFilePath: details.SourceFilePath,
                    sourceLineNumber: details.SourceLineNumber,
                    timeout: details.Timeout)
            };

            return new ValueTask<IReadOnlyCollection<IXunitTestCase>>(testCases);
        }
    }
}
