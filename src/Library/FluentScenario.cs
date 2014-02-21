using System;
using System.Dynamic;
using Kekiri.Impl;
using Kekiri.Reporting;
using NUnit.Framework;

namespace Kekiri
{
    [TestFixture]
    public abstract class FluentScenario : IContextContainer
    {
        private readonly ScenarioRunner _scenarioRunner;
        private StepType _stepType = StepType.Given;

        protected FluentScenario()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            var reportTarget = CreateReportTarget();
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
            _scenarioRunner = new ScenarioRunner(this, reportTarget);
        }
        
        [Test]
        public void RunScenario()
        {
            try
            {
                Before();
                _scenarioRunner.Run();
            }
            finally
            {
                After();
            }
        }

        protected TException Catch<TException>() where TException : Exception
        {
            return _scenarioRunner.Catch<TException>();
        }

        protected void Given(Action action)
        {
            _stepType = StepType.Given;
            AddStepMethod(action);
        }

        protected void Given<TStep>() where TStep : Step
        {
            _stepType = StepType.Given;
            AddStepClass<TStep>();
        }

        protected void When(Action action)
        {
            _stepType = StepType.When;
            AddStepMethod(action);
        }

        protected void When<TStep>() where TStep : Step
        {
            _stepType = StepType.When;
            AddStepClass<TStep>();
        }

        protected void Then(Action action)
        {
            _stepType = StepType.Then;
            AddStepMethod(action);
        }

        protected void Then<TStep>() where TStep : Step
        {
            _stepType = StepType.Then;
            AddStepClass<TStep>();
        }

        protected void And(Action action)
        {
            AddStepMethod(action);
        }

        protected void And<TStep>() where TStep : Step
        {
            AddStepClass<TStep>();
        }

        protected void But(Action action)
        {
            AddStepMethod(action);
        }

        protected void But<TStep>() where TStep : Step
        {
            AddStepClass<TStep>();
        }

        private void AddStepMethod(Action action)
        {
            _scenarioRunner.AddStep(new StepMethodInvoker(_stepType, action.Method));
        }

        private void AddStepClass<TStep>() where TStep : Step
        {
            _scenarioRunner.AddStep(new StepClassInvoker(_stepType, typeof(TStep)));
        }

        private object _context;
        protected internal dynamic Context
        {
            get { return _context ?? (_context = CreateContextObject()); }
        }

        dynamic IContextContainer.Context
        {
            get { return Context; }
        }

        protected virtual void Before() {}
        protected virtual void After() {}

        protected virtual object CreateContextObject()
        {
            return new ExpandoObject();
        }

        protected virtual IReportTarget CreateReportTarget()
        {
            return TraceReportTarget.GetInstance();
        }
    }

    public abstract class FluentScenario<TContext> : FluentScenario where TContext : new()
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