using System;
using Kekiri.Impl;
using Kekiri.Reporting;
using NUnit.Framework;

namespace Kekiri
{
    public abstract class Test
    {
        private ScenarioRunner _scenarioRunner;
        private IReportTarget _reportTarget;

        [TestFixtureSetUp]
        public virtual void SetupScenario()
        {
            _reportTarget = CreateReportTarget();
            _scenarioRunner = new ScenarioRunner(this, _reportTarget);
            foreach (var step in ScenarioMapper.GetStepInvokers(this))
            {
                _scenarioRunner.AddStep(step);
            }
            _scenarioRunner.ReportScenario();
            _scenarioRunner.RunGivens();
            _scenarioRunner.RunWhen();
            _scenarioRunner.EnsureAtLeastOneThenExists();
            //thens are executed by NUnit
        }

        [TestFixtureTearDown]
        public virtual void CleanupScenario()
        {
            _scenarioRunner.AssertExceptionState();
        }

        protected TException Catch<TException>() where TException : Exception
        {
            return _scenarioRunner.Catch<TException>();
        }

        internal virtual IReportTarget CreateReportTarget()
        {
            return CompositeReportTarget.GetInstance();
        }
    }
}