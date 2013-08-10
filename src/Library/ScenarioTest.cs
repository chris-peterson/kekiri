using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kekiri.Exceptions;
using Kekiri.Impl;
using Kekiri.Reporting;
using NUnit.Framework;

namespace Kekiri
{
    public abstract class ScenarioTest
    {
        protected ScenarioTest()
        {
// ReSharper disable DoNotCallOverridableMethodsInConstructor
            _reportTarget = CreateReportTarget();
// ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        private ScenarioTestMetadata _scenario;
        private readonly IReportTarget _reportTarget;

        private Exception _exception;
        private bool _exceptionCaught;

        [TestFixtureSetUp]
        public virtual void SetupScenario()
        {
            _scenario = ScenarioMapper.Map(this);

            if (!_scenario.IsOutputSuppressed)
            {
                _reportTarget.Report(ReportType.EntireScenario, _scenario.CreateReportForEntireScenario());
            }

            ProcessGivens(_scenario.GivenMethods);
            ProcessWhens(_scenario.WhenMethods);
        }

        [SetUp]
        public virtual void SetupTest()
        {
            if (!_scenario.IsOutputSuppressed)
            {
                 _reportTarget.Report(ReportType.CurrentTest, _scenario.CreateReportForCurrentTest());
            }
        }

        [TestFixtureTearDown]
        public virtual void CleanupScenario()
        {
            if (_exception != null && !_exceptionCaught)
            {
                throw new ExpectedExceptionNotCaught(this, _exception);
            }

            var when = _scenario.WhenMethods.Single();
            var throwsAttribute =
                when.GetCustomAttributes(typeof(ThrowsAttribute), false).SingleOrDefault() as ThrowsAttribute;
            if (throwsAttribute != null && _exception == null)
            {
                throw new NoExceptionThrown(this);
            }
        }

        protected TException Catch<TException>() where TException : Exception
        {
            if (_exception == null)
            {
                throw new NoExceptionThrown(this, typeof(TException));
            }

            var exception = _exception as TException;
            if (exception == null)
            {
                throw new WrongExceptionType(this, typeof(TException), _exception);
            }

            _exceptionCaught = true;
            return exception;
        }

        protected virtual IReportTarget CreateReportTarget()
        {
            return TraceReportTarget.GetInstance();
        }

        private void ProcessGivens(IEnumerable<MethodBase> givens)
        {
            foreach (var given in givens)
            {
                try
                {
                    InvokeStepMethod(given);
                }
                catch (TargetInvocationException ex)
                {
                    throw new GivenFailed(this, given, ex.InnerException);
                }
            }
        }

        private void ProcessWhens(IEnumerable<MethodBase> whenMethods)
        {
            var when = whenMethods.Single();

            try
            {
                InvokeStepMethod(when);
            }
            catch (TargetInvocationException ex)
            {
                var exceptionWasExpected = when.GetCustomAttributes(typeof(ThrowsAttribute), false).SingleOrDefault() != null;
                if (exceptionWasExpected)
                {
                    _exception = ex.InnerException;
                }
                else
                {
                    throw new WhenFailed(this, when, ex.InnerException);
                }
            }
        }

        private void InvokeStepMethod(MethodBase stepMethod)
        {
            stepMethod.Invoke(stepMethod.IsStatic ? null : this, null);
        }
    }
}