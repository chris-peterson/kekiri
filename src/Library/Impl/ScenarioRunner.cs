using System;
using System.Linq;
using System.Reflection;
using Kekiri.Impl.Exceptions;
using Kekiri.Impl.Reporting;

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
        readonly ScenarioBase _scenario;
        readonly ScenarioTestMetadata _scenarioMetadata;
        readonly IReportTarget _reportTarget;
        Exception _exception;
        bool _exceptionCaught;

        public ScenarioRunner(ScenarioBase scenario, IReportTarget reportTarget)
        {
            _scenario = scenario;
            _scenarioMetadata = new ScenarioTestMetadata(scenario.GetType());
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
                throw new NoExceptionThrown(_scenario, typeof(TException));
            }

            var exception = _exception as TException;
            if (exception == null)
            {
                throw new WrongExceptionType(_scenario, typeof(TException), _exception);
            }

            _exceptionCaught = true;
            return exception;
        }

        public void AssertExceptionCompliance()
        {
            if (_exception != null && !_exceptionCaught)
            {
                throw new ExpectedExceptionNotCaught(_scenario, _exception);
            }

            if (_scenarioMetadata.WhenMethod.ExceptionExpected && _exception == null)
            {
                throw new NoExceptionThrown(_scenario);
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
                throw new FixtureShouldHaveThens(_scenario);
        }

        public void RunGivens()
        {
            foreach (var given in _scenarioMetadata.GivenMethods)
            {
                try
                {
                    given.Invoke(_scenario);
                }
                catch (TargetInvocationException ex)
                {
                    throw new GivenFailed(_scenario, given.Name.PrettyName, ex.InnerException);
                }
            }
        }

        public void RunWhen()
        {
            var when = _scenarioMetadata.WhenMethod;
            if (when == null)
            {
                throw new FixtureShouldHaveWhens(_scenario);
            }
            try
            {
                when.Invoke(_scenario);
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
                    throw new WhenFailed(_scenario, when.Name.PrettyName, _exception);
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
                    given.Invoke(_scenario);
                }
                catch (TargetInvocationException ex)
                {
                    throw new ThenFailed(_scenario, given.Name.PrettyName, ex.InnerException);
                }
            }
        }
    }
}