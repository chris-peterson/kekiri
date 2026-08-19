using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace Kekiri.Xunit.Infrastructure
{
    static class ScenarioTestCaseFactory
    {
        public static void GuardScenarioClass(IXunitTestMethod testMethod, string attributeName)
        {
            if (!typeof(ScenarioBase).IsAssignableFrom(testMethod.TestClass.Class))
            {
                throw new NotSupportedException(
                    $"The {attributeName.Replace("Attribute", string.Empty)} attribute can only be placed on a class inheriting from Kekiri.Xunit.Scenarios");
            }
        }

        // v2 asked xunit for TestMethodDisplayOptions.ReplaceUnderscoreWithSpace per test case; v3 reads
        // that from runner config, so the name is built here to keep scenario titles readable by default.
        public static string DisplayName(IXunitTestMethod testMethod) =>
            testMethod.MethodName.Replace('_', ' ');

        public static Dictionary<string, HashSet<string>> ToReadWrite(
            IReadOnlyDictionary<string, IReadOnlyCollection<string>> traits)
        {
            var result = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

            foreach (var trait in traits)
            {
                result[trait.Key] = new HashSet<string>(trait.Value);
            }

            return result;
        }

        /// <summary>
        /// Mirrors XunitRunnerHelper.RunXunitTestCase, but dispatches to <see cref="ScenarioTestCaseRunner"/>
        /// so the recorded Given/When/Then chain is executed after the test method body returns.
        /// </summary>
        public static async ValueTask<RunSummary> Run(
            IXunitTestCase testCase,
            ExplicitOption explicitOption,
            IMessageBus messageBus,
            object[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource,
            ParallelMode parallelMode,
            ExecutionScheduler scheduler,
            FixtureMappingManager methodFixtureMappings)
        {
            var tests = await aggregator.RunAsync(testCase.CreateTests, new IXunitTest[0]);

            if (aggregator.ToException() is Exception ex)
            {
                if (ex.Message != null && ex.Message.StartsWith(DynamicSkipToken.Value, StringComparison.Ordinal))
                {
                    return XunitRunnerHelper.SkipTestCases(
                        messageBus,
                        cancellationTokenSource,
                        new[] { testCase },
                        ex.Message.Substring(DynamicSkipToken.Value.Length));
                }

                if (testCase.SkipExceptions != null && testCase.SkipExceptions.Contains(ex.GetType()))
                {
                    return XunitRunnerHelper.SkipTestCases(
                        messageBus,
                        cancellationTokenSource,
                        new[] { testCase },
                        string.IsNullOrEmpty(ex.Message)
                            ? $"Exception of type '{ex.GetType().FullName}' was thrown"
                            : ex.Message);
                }

                return XunitRunnerHelper.FailTestCases(
                    messageBus,
                    cancellationTokenSource,
                    new[] { testCase },
                    ex);
            }

            return await ScenarioTestCaseRunner.Instance.Run(
                testCase,
                tests,
                messageBus,
                aggregator,
                cancellationTokenSource,
                parallelMode,
                scheduler,
                testCase.TestCaseDisplayName,
                testCase.SkipReason,
                explicitOption,
                constructorArguments,
                methodFixtureMappings);
        }
    }
}
