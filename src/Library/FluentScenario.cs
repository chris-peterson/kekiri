using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Kekiri.Impl;
using Kekiri.Reporting;
using NUnit.Framework;

namespace Kekiri
{
    [TestFixture]
    public abstract class FluentScenario : IContextAccessor
    {
        private readonly ScenarioRunner _scenarioRunner;
        private readonly string[] _parameterNames;
        private StepType _stepType = StepType.Given;

        protected FluentScenario()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            var reportTarget = CreateReportTarget();
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
            _parameterNames = ScenarioMapper.GetParameterNames(this);
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

        #region Given

        protected void Given(Action action)
        {
            _stepType = StepType.Given;
            AddStepMethod(action);
        }

        protected void Given<T>(Action<T> action, T a)
        {
            _stepType = StepType.Given;
            AddStepMethod(action.Method, a);
        }

        protected void Given<T1, T2>(Action<T1, T2> action, T1 a, T2 b)
        {
            _stepType = StepType.Given;
            AddStepMethod(action.Method, a, b);
        }

        protected void Given<T1, T2, T3>(Action<T1, T2, T3> action, T1 a, T2 b, T3 c)
        {
            _stepType = StepType.Given;
            AddStepMethod(action.Method, a, b, c);
        }

        protected void Given<TStep>() where TStep : Step
        {
            _stepType = StepType.Given;
            AddStepClass<TStep>();
        }

        #endregion

        #region When

        protected void When(Action action)
        {
            _stepType = StepType.When;
            AddStepMethod(action);
        }

        protected void When<T>(Action<T> action, T a)
        {
            _stepType = StepType.When;
            AddStepMethod(action.Method, a);
        }

        protected void When<T1, T2>(Action<T1, T2> action, T1 a, T2 b)
        {
            _stepType = StepType.When;
            AddStepMethod(action.Method, a, b);
        }

        protected void When<T1, T2, T3>(Action<T1, T2, T3> action, T1 a, T2 b, T3 c)
        {
            _stepType = StepType.When;
            AddStepMethod(action.Method, a, b, c);
        }

        protected void When<TStep>() where TStep : Step
        {
            _stepType = StepType.When;
            AddStepClass<TStep>();
        }

        #endregion

        #region Then

        protected void Then(Action action)
        {
            _stepType = StepType.Then;
            AddStepMethod(action);
        }

        protected void Then<T>(Action<T> action, T a)
        {
            _stepType = StepType.Then;
            AddStepMethod(action.Method, a);
        }

        protected void Then<T1, T2>(Action<T1, T2> action, T1 a, T2 b)
        {
            _stepType = StepType.Then;
            AddStepMethod(action.Method, a, b);
        }

        protected void Then<T1, T2, T3>(Action<T1, T2, T3> action, T1 a, T2 b, T3 c)
        {
            _stepType = StepType.Then;
            AddStepMethod(action.Method, a, b, c);
        }

        protected void Then<TStep>() where TStep : Step
        {
            _stepType = StepType.Then;
            AddStepClass<TStep>();
        }

        #endregion

        #region And

        protected void And(Action action)
        {
            AddStepMethod(action);
        }

        protected void And<T>(Action<T> action, T a)
        {
            AddStepMethod(action.Method, a);
        }

        protected void And<T1, T2>(Action<T1, T2> action, T1 a, T2 b)
        {
            AddStepMethod(action.Method, a, b);
        }

        protected void And<T1, T2, T3>(Action<T1, T2, T3> action, T1 a, T2 b, T3 c)
        {
            AddStepMethod(action.Method, a, b, c);
        }

        protected void And<TStep>() where TStep : Step
        {
            AddStepClass<TStep>();
        }

        #endregion

        #region But

        protected void But(Action action)
        {
            AddStepMethod(action);
        }

        protected void But<T>(Action<T> action, T a)
        {
            AddStepMethod(action.Method, a);
        }

        protected void But<T1, T2>(Action<T1, T2> action, T1 a, T2 b)
        {
            AddStepMethod(action.Method, a, b);
        }

        protected void But<T1, T2, T3>(Action<T1, T2, T3> action, T1 a, T2 b, T3 c)
        {
            AddStepMethod(action.Method, a, b, c);
        }

        protected void But<TStep>() where TStep : Step
        {
            AddStepClass<TStep>();
        }

        #endregion

        private void AddStepMethod(Action action)
        {
            _scenarioRunner.AddStep(new StepMethodInvoker(_stepType, action.Method));
        }

        private void AddStepMethod(MethodInfo method, params object[] parameterValues)
        {
            var parameters = ExtractParameters(method, parameterValues);
            _scenarioRunner.AddStep(new StepMethodInvoker(_stepType, method, parameters));
        }

        private void AddStepClass<TStep>() where TStep : Step
        {
            _scenarioRunner.AddStep(new StepClassInvoker(_stepType, typeof(TStep), new Dictionary<string, object>()));
        }

        private KeyValuePair<string, object>[] ExtractParameters(MethodInfo method, object[] parameterValues)
        {
            return method.GetParameters()
                .Select((p, index) => new KeyValuePair<string, object>(p.Name, parameterValues[index]))
                .ToArray();
        }

        private object _context;
        protected internal dynamic Context
        {
            get { return _context ?? (_context = CreateContextObject()); }
        }

        dynamic IContextAccessor.Context
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