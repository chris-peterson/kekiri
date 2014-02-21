using System;
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

        private ScenarioRunner _scenarioRunner;
        private readonly IReportTarget _reportTarget;

        [TestFixtureSetUp]
        public virtual void SetupScenario()
        {
            _scenarioRunner = new ScenarioRunner(this, _reportTarget);
            foreach (var step in ScenarioMapper.GetStepInvokers(GetType()))
            {
                _scenarioRunner.AddStep(step);
            }
            _scenarioRunner.ReportScenario();
            _scenarioRunner.RunGivensAndWhen();
            _scenarioRunner.EnsureAtLeastOneThenExists();
        }

        [SetUp]
        public virtual void SetupTest()
        {
            _scenarioRunner.ReportCurrentTest();
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
        
        protected virtual IReportTarget CreateReportTarget()
        {
            return TraceReportTarget.GetInstance();
        }
    }
}