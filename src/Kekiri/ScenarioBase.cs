using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Kekiri.Impl;
using Kekiri.Impl.Exceptions;
using Kekiri.Impl.Reporting;
using Kekiri.IoC;

namespace Kekiri
{
    public abstract class ScenarioBase
    {
        readonly IReportTarget _reportTarget;
        ScenarioRunner _scenarioRunner;

        protected ScenarioBase()
        {
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            _reportTarget = CreateReportTarget();
            Initialize();
        }

        public void Initialize()
        {
            _scenarioRunner = new ScenarioRunner(this, _reportTarget);
        }

        public virtual async Task RunAsync()
        {
            try
            {
                await BeforeAsync();
                await _scenarioRunner.RunAsync();
            }
            finally
            {
                await AfterAsync();
            }
        }

        protected TException Catch<TException>() where TException : Exception
        {
            return _scenarioRunner.Catch<TException>();
        }

        public static Func<Container> ContainerFactory { get; internal set; }

        Container _container;
        public Container Container
        {
            get
            {
                if (ContainerFactory == null)
                {
                    throw new ContainerFactoryNotInitialized(this);
                }

                return _container ?? (_container = ContainerFactory.Invoke());
            }
        }

        #region Base
        // shared by nestable steps (e.g. Given/Then) and steps that can't be nested (e.g. When)
        public abstract class StepOptionsBase
        {
            protected readonly ScenarioBase Scenario;

            protected StepOptionsBase(ScenarioBase scenario)
            {
                Scenario = scenario;
            }

            internal abstract StepType StepType { get; }
        }

        public abstract class NestedStepOptions : StepOptionsBase
        {
            protected NestedStepOptions(ScenarioBase scenario) : base(scenario)
            {
            }

            #region And
            public NestedStepOptions And(Action action)
            {
                Scenario.AddStepMethod(StepType, action);
                return this;
            }
            
            public NestedStepOptions AndAsync(Func<Task> action)
            {
                Scenario.AddStepMethod(StepType, action);
                return this;
            }

            public NestedStepOptions And<T>(Action<T> action, T a)
            {
                Scenario.AddStepMethod(StepType, action.GetMethodInfo(), a);
                return this;
            }
            
            public NestedStepOptions AndAsync<T>(Func<T, Task> action, T a)
            {
                Scenario.AddStepMethod(StepType, action.GetMethodInfo(), a);
                return this;
            }

            public NestedStepOptions And<T1, T2>(Action<T1, T2> action, T1 a, T2 b)
            {
                Scenario.AddStepMethod(StepType, action.GetMethodInfo(), a, b);
                return this;
            }
            
            public NestedStepOptions AndAsync<T1, T2>(Func<T1, T2, Task> action, T1 a, T2 b)
            {
                Scenario.AddStepMethod(StepType, action.GetMethodInfo(), a, b);
                return this;
            }

            public NestedStepOptions And<T1, T2, T3>(Action<T1, T2, T3> action, T1 a, T2 b, T3 c)
            {
                Scenario.AddStepMethod(StepType, action.GetMethodInfo(), a, b, c);
                return this;
            }
            
            public NestedStepOptions AndAsync<T1, T2, T3>(Func<T1, T2, T3, Task> action, T1 a, T2 b, T3 c)
            {
                Scenario.AddStepMethod(StepType, action.GetMethodInfo(), a, b, c);
                return this;
            }

            public NestedStepOptions And<TStep>(params object[] parameterValues) where TStep : Step
            {
                Scenario.AddStepClass<TStep>(StepType, parameterValues);
                return this;
            }
            #endregion

            #region But
            public NestedStepOptions But(Action action)
            {
                Scenario.AddStepMethod(StepType, action);
                return this;
            }
            
            public NestedStepOptions ButAsync(Func<Task> action)
            {
                Scenario.AddStepMethod(StepType, action);
                return this;
            }

            public NestedStepOptions But<T>(Action<T> action, T a)
            {
                Scenario.AddStepMethod(StepType, action.GetMethodInfo(), a);
                return this;
            }
            
            public NestedStepOptions ButAsync<T>(Func<T, Task> action, T a)
            {
                Scenario.AddStepMethod(StepType, action.GetMethodInfo(), a);
                return this;
            }

            public NestedStepOptions But<T1, T2>(Action<T1, T2> action, T1 a, T2 b)
            {
                Scenario.AddStepMethod(StepType, action.GetMethodInfo(), a, b);
                return this;
            }
            
            public NestedStepOptions ButAsync<T1, T2>(Func<T1, T2, Task> action, T1 a, T2 b)
            {
                Scenario.AddStepMethod(StepType, action.GetMethodInfo(), a, b);
                return this;
            }

            public NestedStepOptions But<T1, T2, T3>(Action<T1, T2, T3> action, T1 a, T2 b, T3 c)
            {
                Scenario.AddStepMethod(StepType, action.GetMethodInfo(), a, b, c);
                return this;
            }
            
            public NestedStepOptions ButAsync<T1, T2, T3>(Func<T1, T2, T3, Task> action, T1 a, T2 b, T3 c)
            {
                Scenario.AddStepMethod(StepType, action.GetMethodInfo(), a, b, c);
                return this;
            }

            public NestedStepOptions But<TStep>(params object[] parameterValues) where TStep : Step
            {
                Scenario.AddStepClass<TStep>(StepType, parameterValues);
                return this;
            }
            #endregion
        }
        #endregion

        #region Given
        public class GivenOptions : NestedStepOptions
        {
            public GivenOptions(ScenarioBase scenario)
                : base(scenario)
            {
            }

            internal override StepType StepType => StepType.Given;
        }

        protected GivenOptions Given(Action action)
        {
            var given = new GivenOptions(this);
            given.And(action);
            return given;
        }
        
        protected GivenOptions GivenAsync(Func<Task> action)
        {
            var given = new GivenOptions(this);
            given.AndAsync(action);
            return given;
        }

        protected GivenOptions Given<T>(Action<T> action, T a)
        {
            var given = new GivenOptions(this);
            given.And(action, a);
            return given;
        }
        
        protected GivenOptions GivenAsync<T>(Func<T, Task> action, T a)
        {
            var given = new GivenOptions(this);
            given.AndAsync(action, a);
            return given;
        }

        protected GivenOptions Given<T1, T2>(Action<T1, T2> action, T1 a, T2 b)
        {
            var given = new GivenOptions(this);
            given.And(action, a, b);
            return given;
        }
        
        protected GivenOptions GivenAsync<T1, T2>(Func<T1, T2, Task> action, T1 a, T2 b)
        {
            var given = new GivenOptions(this);
            given.AndAsync(action, a, b);
            return given;
        }

        protected GivenOptions Given<T1, T2, T3>(Action<T1, T2, T3> action, T1 a, T2 b, T3 c)
        {
            var given = new GivenOptions(this);
            given.And(action, a, b, c);
            return given;
        }
        
        protected GivenOptions GivenAsync<T1, T2, T3>(Func<T1, T2, T3, Task> action, T1 a, T2 b, T3 c)
        {
            var given = new GivenOptions(this);
            given.AndAsync(action, a, b, c);
            return given;
        }

        protected GivenOptions Given<TStep>(params object[] parameterValues) where TStep : Step
        {
            var given = new GivenOptions(this);
            given.And<TStep>(parameterValues);
            return given;
        }
        #endregion

        #region When
        public class WhenOptions : StepOptionsBase
        {
            public WhenOptions(ScenarioBase scenario) : base(scenario)
            {
            }

            internal override StepType StepType => StepType.When;

            public void Throws()
            {
                Scenario._scenarioRunner.ExpectException();
            }

            internal WhenOptions That(Action action)
            {
                Scenario.AddStepMethod(StepType, action);
                return this;
            }
            
            internal WhenOptions ThatAsync(Func<Task> action)
            {
                Scenario.AddStepMethod(StepType, action);
                return this;
            }

            internal WhenOptions That<T>(Action<T> action, T a)
            {
                Scenario.AddStepMethod(StepType, action.GetMethodInfo(), a);
                return this;
            }
            
            internal WhenOptions ThatAsync<T>(Func<T, Task> action, T a)
            {
                Scenario.AddStepMethod(StepType, action.GetMethodInfo(), a);
                return this;
            }

            internal WhenOptions That<T1, T2>(Action<T1, T2> action, T1 a, T2 b)
            {
                Scenario.AddStepMethod(StepType, action.GetMethodInfo(), a, b);
                return this;
            }
            
            internal WhenOptions ThatAsync<T1, T2>(Func<T1, T2, Task> action, T1 a, T2 b)
            {
                Scenario.AddStepMethod(StepType, action.GetMethodInfo(), a, b);
                return this;
            }

            internal WhenOptions That<T1, T2, T3>(Action<T1, T2, T3> action, T1 a, T2 b, T3 c)
            {
                Scenario.AddStepMethod(StepType, action.GetMethodInfo(), a, b, c);
                return this;
            }
            
            internal WhenOptions ThatAsync<T1, T2, T3>(Func<T1, T2, T3, Task> action, T1 a, T2 b, T3 c)
            {
                Scenario.AddStepMethod(StepType, action.GetMethodInfo(), a, b, c);
                return this;
            }

            internal WhenOptions That<TStep>(params object[] parameterValues) where TStep : Step
            {
                Scenario.AddStepClass<TStep>(StepType, parameterValues);
                return this;
            }
        }

        protected WhenOptions When(Action action)
        {
            return new WhenOptions(this).That(action);
        }
        
        protected WhenOptions WhenAsync(Func<Task> action)
        {
            return new WhenOptions(this).ThatAsync(action);
        }

        protected WhenOptions When<T>(Action<T> action, T a)
        {
            return new WhenOptions(this).That(action, a);
        }
        
        protected WhenOptions WhenAsync<T>(Func<T, Task> action, T a)
        {
            return new WhenOptions(this).ThatAsync(action, a);
        }

        protected WhenOptions When<T1, T2>(Action<T1, T2> action, T1 a, T2 b)
        {
            return new WhenOptions(this).That(action, a, b);
        }
        
        protected WhenOptions WhenAsync<T1, T2>(Func<T1, T2, Task> action, T1 a, T2 b)
        {
            return new WhenOptions(this).ThatAsync(action, a, b);
        }

        protected WhenOptions When<T1, T2, T3>(Action<T1, T2, T3> action, T1 a, T2 b, T3 c)
        {
            return new WhenOptions(this).That(action, a, b, c);
        }
        
        protected WhenOptions WhenAsync<T1, T2, T3>(Func<T1, T2, T3, Task> action, T1 a, T2 b, T3 c)
        {
            return new WhenOptions(this).ThatAsync(action, a, b, c);
        }

        protected WhenOptions When<TStep>(params object[] parameterValues) where TStep : Step
        {
            return new WhenOptions(this).That<TStep>(parameterValues);
        }
        #endregion

        #region Then
        public class ThenOptions : NestedStepOptions
        {
            public ThenOptions(ScenarioBase scenario)
                : base(scenario)
            {
            }

            internal override StepType StepType => StepType.Then;
        }

        protected ThenOptions Then(Action action)
        {
            var then = new ThenOptions(this);
            then.And(action);
            return then;
        }
        
        protected ThenOptions ThenAsync(Func<Task> action)
        {
            var then = new ThenOptions(this);
            then.AndAsync(action);
            return then;
        }

        protected ThenOptions Then<T>(Action<T> action, T a)
        {
            var then = new ThenOptions(this);
            then.And(action, a);
            return then;
        }

        protected ThenOptions ThenAsync<T>(Func<T, Task> action, T a)
        {
            var then = new ThenOptions(this);
            then.AndAsync(action, a);
            return then;
        }
        
        protected ThenOptions Then<T1, T2>(Action<T1, T2> action, T1 a, T2 b)
        {
            var then = new ThenOptions(this);
            then.And(action, a, b);
            return then;
        }
        
        protected ThenOptions ThenAsync<T1, T2>(Func<T1, T2, Task> action, T1 a, T2 b)
        {
            var then = new ThenOptions(this);
            then.AndAsync(action, a, b);
            return then;
        }

        protected ThenOptions Then<T1, T2, T3>(Action<T1, T2, T3> action, T1 a, T2 b, T3 c)
        {
            var then = new ThenOptions(this);
            then.And(action, a, b, c);
            return then;
        }

        protected ThenOptions ThenAsync<T1, T2, T3>(Func<T1, T2, T3, Task> action, T1 a, T2 b, T3 c)
        {
            var then = new ThenOptions(this);
            then.AndAsync(action, a, b, c);
            return then;
        }
        
        protected ThenOptions Then<TStep>(params object[] parameterValues) where TStep : Step
        {
            var then = new ThenOptions(this);
            then.And<TStep>(parameterValues);
            return then;
        }
        #endregion

        void AddStepMethod(StepType stepType, Action action)
        {
            _scenarioRunner.AddStep(new StepMethodInvoker(stepType, action.GetMethodInfo()));
        }
        
        void AddStepMethod(StepType stepType, Func<Task> action)
        {
            _scenarioRunner.AddStep(new StepMethodInvoker(stepType, action.GetMethodInfo()));
        }

        void AddStepMethod(StepType stepType, MethodBase method, params object[] parameterValues)
        {
            var parameters = ExtractParameters(method, parameterValues);
            _scenarioRunner.AddStep(new StepMethodInvoker(stepType, method, parameters));
        }

        void AddStepClass<TStep>(StepType stepType, params object[] parameterValues) where TStep : Step
        {
            var stepClass = typeof(TStep);
            var ctor = stepClass.GetTypeInfo().GetConstructors()
                .SingleOrDefault();
            if (ctor == null)
            {
                throw new ConstructorNotFound(this, $"Could not find a constructor for {stepType} {stepClass.Name} ({string.Join(", ", parameterValues.Select(p => $"{p.GetType().Name} {p}"))})");
            }
            var parameters = ExtractParameters(ctor, parameterValues);
            _scenarioRunner.AddStep(new StepClassInvoker(stepType, stepClass, parameters, _scenarioRunner));
        }

        static KeyValuePair<string, object>[] ExtractParameters(MethodBase method, object[] parameterValues)
        {
            return method.GetParameters()
                .Select((p, index) => new KeyValuePair<string, object>(p.Name, parameterValues[index]))
                .ToArray();
        }

        object _context;

        protected internal dynamic Context => _context ?? (_context = CreateContextObject());

        protected virtual Task BeforeAsync() => Task.CompletedTask;

        protected virtual Task AfterAsync() => Task.CompletedTask;

        protected virtual object CreateContextObject()
        {
            return new ExpandoObject();
        }

        internal virtual IReportTarget CreateReportTarget()
        {
            return CompositeReportTarget.GetInstance();
        }
    }

    public abstract class ScenarioBase<TContext> : ScenarioBase
    {
        protected override object CreateContextObject()
        {
            var contextType = typeof (TContext);
            var ctor = contextType.GetTypeInfo().GetConstructor(new Type[] {});
            if (ctor != null)
                return ctor.Invoke(null);

            var method = typeof (Container).GetTypeInfo().GetMethod("Resolve").MakeGenericMethod(contextType);
            return method.Invoke(Container, null);
        }

        protected internal new TContext Context => base.Context;
    }
}