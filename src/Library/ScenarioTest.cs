using System;
using System.Collections.Generic;
using System.Dynamic;
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
            if (when.ExceptionExpected && _exception == null)
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

        private object _context;
        protected internal dynamic Context
        {
            get { return _context ?? (_context = CreateContextObject()); }
        }

        protected virtual object CreateContextObject()
        {
            return new ExpandoObject();
        }

        protected virtual IReportTarget CreateReportTarget()
        {
            return TraceReportTarget.GetInstance();
        }

        private void ProcessGivens(IEnumerable<IStep> givens)
        {
            foreach (var given in givens)
            {
                try
                {
                    given.Invoke(this);
                }
                catch (TargetInvocationException ex)
                {
                    throw new GivenFailed(this, given.Name, ex.InnerException);
                }
            }
        }

        private void ProcessWhens(IEnumerable<IStep> whenMethods)
        {
            var when = whenMethods.Single();

            try
            {
                when.Invoke(this);
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
                    throw new WhenFailed(this, when.Name, ex.InnerException);
                }
            }
        }
    }

    public class ScenarioTest<TContext> : ScenarioTest where TContext : new()
    {
        protected override object CreateContextObject()
        {
            return new TContext();
        }

        protected internal new TContext Context
        {
            get { return base.Context; }
        }
    }
}