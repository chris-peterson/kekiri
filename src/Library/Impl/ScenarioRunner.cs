using System;
using System.Linq;
using System.Reflection;
using Kekiri.Exceptions;
using Kekiri.Reporting;

namespace Kekiri.Impl
{
    internal class ScenarioRunner
    {
        private readonly object _test;
        private readonly ScenarioTestMetadata _scenarioMetadata;
        private readonly IReportTarget _reportTarget;
        private Exception _exception;
        private bool _exceptionCaught;

        public ScenarioRunner(object test, IReportTarget reportTarget)
        {
            _test = test;
            _scenarioMetadata = BuildScenarioMetadata(test);
            _reportTarget = reportTarget;
        }

        public void AddStep(IStepInvoker step)
        {
            _scenarioMetadata.AddStep(step);
        }

        public TException Catch<TException>() where TException : Exception
        {
            if (_exception == null)
            {
                throw new NoExceptionThrown(_test, typeof(TException));
            }

            var exception = _exception as TException;
            if (exception == null)
            {
                throw new WrongExceptionType(_test, typeof(TException), _exception);
            }

            _exceptionCaught = true;
            return exception;
        }

        public void Run()
        {
            ReportScenario();
            InvokeGivens();
            InvokeWhen();
            InvokeThens();
            AssertExceptionState();
        }

        public void ReportScenario()
        {
            if (!_scenarioMetadata.IsOutputSuppressed)
                _reportTarget.Report(ReportType.EntireScenario, _scenarioMetadata.CreateReportForEntireScenario());
        }

        public void ReportCurrentTest()
        {
            if (!_scenarioMetadata.IsOutputSuppressed)
            {
                _reportTarget.Report(ReportType.CurrentTest, _scenarioMetadata.CreateReportForCurrentTest());
            }
        }

        public void RunGivensAndWhen()
        {
            InvokeGivens();
            InvokeWhen();
        }

        public void AssertExceptionState()
        {
            if (_exception != null && !_exceptionCaught)
            {
                throw new ExpectedExceptionNotCaught(_test, _exception);
            }

            if (_scenarioMetadata.WhenMethod.ExceptionExpected && _exception == null)
            {
                throw new NoExceptionThrown(_test);
            }
        }

        public void EnsureAtLeastOneThenExists()
        {
            if (!_scenarioMetadata.ThenMethods.Any())
                throw new FixtureShouldHaveThens(_test);
        }

        private void InvokeGivens()
        {
            foreach (var given in _scenarioMetadata.GivenMethods)
            {
                try
                {
                    given.Invoke(_test);
                }
                catch (TargetInvocationException ex)
                {
                    throw new GivenFailed(_test, given.Name.PrettyName, ex.InnerException);
                }
            }
        }

        private void InvokeWhen()
        {
            var when = _scenarioMetadata.WhenMethod;
            if (when == null)
            {
                throw new FixtureShouldHaveWhens(_test);
            }
            try
            {
                when.Invoke(_test);
            }
            catch (TargetInvocationException ex)
            {
                var exceptionWasExpected = when.ExceptionExpected;
                if (exceptionWasExpected)
                {
                    _exception = ex.InnerException;
                }
                else
                {
                    throw new WhenFailed(_test, when.Name.PrettyName, ex.InnerException);
                }
            }
        }

        private void InvokeThens()
        {
            EnsureAtLeastOneThenExists();
            
            foreach (var given in _scenarioMetadata.ThenMethods)
            {
                try
                {
                    given.Invoke(_test);
                }
                catch (TargetInvocationException ex)
                {
                    throw new ThenFailed(_test, given.Name.PrettyName, ex.InnerException);
                }
            }
        }

        private ScenarioTestMetadata BuildScenarioMetadata(object test)
        {
            var scenarioMetadata = new ScenarioTestMetadata(test.GetType());
            ScenarioMapper.FillParameters(test, scenarioMetadata.Parameters);
            return scenarioMetadata;
        }
    }
}