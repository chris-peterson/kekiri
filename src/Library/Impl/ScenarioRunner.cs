using System;
using System.Linq;
using System.Reflection;
using Kekiri.Exceptions;
using Kekiri.Reporting;

namespace Kekiri.Impl
{
    interface IExceptionHandler
    {
        void ExpectException();
        TException Catch<TException>() where TException : Exception;
        void AssertExceptionCompliance();
    }

    class ScenarioRunner : IExceptionHandler
    {
        readonly object _test;
        readonly ScenarioTestMetadata _scenarioMetadata;
        readonly IReportTarget _reportTarget;
        Exception _exception;
        bool _exceptionCaught;

        public ScenarioRunner(object test, IReportTarget reportTarget)
        {
            _test = test;
            _scenarioMetadata = new ScenarioTestMetadata(test.GetType());
            _reportTarget = reportTarget;
        }

        public void AddStep(IStepInvoker step)
        {
            _scenarioMetadata.AddStep(step);
        }

        #region IExceptionHandler Members
        public void ExpectException()
        {
            _scenarioMetadata.WhenMethod.ExceptionExpected = true;
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

        public void AssertExceptionCompliance()
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
        #endregion

        public void Run()
        {
            ReportScenario();
            RunGivens();
            RunWhen();
            InvokeThens();
            AssertExceptionCompliance();
        }

        public void ReportScenario()
        {
             _reportTarget.Report(_scenarioMetadata.CreateReport());
        }

        public void EnsureAtLeastOneThenExists()
        {
            if (!_scenarioMetadata.ThenMethods.Any())
                throw new FixtureShouldHaveThens(_test);
        }

        public void RunGivens()
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

        public void RunWhen()
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
            catch (Exception ex)
            {
                if (ex is TargetInvocationException)
                {
                    _exception = ex.InnerException;
                }
                else
                {
                    _exception = ex;
                }

                if (!when.ExceptionExpected)
                {
                    throw new WhenFailed(_test, when.Name.PrettyName, _exception);
                }
            }
        }

        void InvokeThens()
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
    }
}