using System;
using System.Threading.Tasks;
using static Kekiri.ScenarioBase;

namespace Kekiri.NUnit
{
    public class FeatureBase
    {
        NUnitScenario scenario;

        public void Initialize()
        {
            Scenario = new NUnitScenario { StepsCallerInstance = this };
        }

        internal NUnitScenario Scenario { get => scenario; set => scenario = value; }

        public virtual Task RunAsync()
        {
            return Scenario.RunAsync();
        }

        protected TException Catch<TException>() where TException : Exception
        {
            return Scenario.Catch<TException>();
        }

        protected GivenOptions Given(Action action)
        {
            return Scenario.Given(action);
        }

        protected GivenOptions GivenAsync(Func<Task> action)
        {
            return Scenario.GivenAsync(action);
        }

        protected GivenOptions Given<T>(Action<T> action, T a)
        {
            return Scenario.Given(action, a);
        }

        protected GivenOptions GivenAsync<T>(Func<T, Task> action, T a)
        {
            return Scenario.GivenAsync(action, a);
        }

        protected GivenOptions Given<T1, T2>(Action<T1, T2> action, T1 a, T2 b)
        {
            return Scenario.Given(action, a, b);
        }

        protected GivenOptions GivenAsync<T1, T2>(Func<T1, T2, Task> action, T1 a, T2 b)
        {
            return Scenario.GivenAsync(action, a, b);
        }

        protected GivenOptions Given<T1, T2, T3>(Action<T1, T2, T3> action, T1 a, T2 b, T3 c)
        {
            return Scenario.Given(action, a, b, c);
        }

        protected GivenOptions GivenAsync<T1, T2, T3>(Func<T1, T2, T3, Task> action, T1 a, T2 b, T3 c)
        {
            return Scenario.GivenAsync(action, a, b, c);
        }

        protected GivenOptions Given<TStep>(params object[] parameterValues) where TStep : Step
        {
            return Scenario.Given<TStep>(parameterValues);
        }

        protected WhenOptions When(Action action)
        {
            return Scenario.When(action);
        }

        protected WhenOptions WhenAsync(Func<Task> action)
        {
            return Scenario.WhenAsync(action);
        }

        protected WhenOptions When<T>(Action<T> action, T a)
        {
            return Scenario.When(action, a);
        }

        protected WhenOptions WhenAsync<T>(Func<T, Task> action, T a)
        {
            return Scenario.WhenAsync(action, a);
        }

        protected WhenOptions When<T1, T2>(Action<T1, T2> action, T1 a, T2 b)
        {
            return Scenario.When(action, a, b);
        }

        protected WhenOptions WhenAsync<T1, T2>(Func<T1, T2, Task> action, T1 a, T2 b)
        {
            return Scenario.WhenAsync(action, a, b);
        }

        protected WhenOptions When<T1, T2, T3>(Action<T1, T2, T3> action, T1 a, T2 b, T3 c)
        {
            return Scenario.When(action, a, b, c);
        }

        protected WhenOptions WhenAsync<T1, T2, T3>(Func<T1, T2, T3, Task> action, T1 a, T2 b, T3 c)
        {
            return Scenario.WhenAsync(action, a, b, c);
        }

        protected WhenOptions When<TStep>(params object[] parameterValues) where TStep : Step
        {
            return Scenario.When<TStep>(parameterValues);
        }

        protected ThenOptions Then(Action action)
        {
            return Scenario.Then(action);
        }

        protected ThenOptions ThenAsync(Func<Task> action)
        {
            return Scenario.ThenAsync(action);
        }

        protected ThenOptions Then<T>(Action<T> action, T a)
        {
            return Scenario.Then(action, a);
        }

        protected ThenOptions ThenAsync<T>(Func<T, Task> action, T a)
        {
            return Scenario.ThenAsync(action, a);
        }

        protected ThenOptions Then<T1, T2>(Action<T1, T2> action, T1 a, T2 b)
        {
            return Scenario.Then(action, a, b);
        }

        protected ThenOptions ThenAsync<T1, T2>(Func<T1, T2, Task> action, T1 a, T2 b)
        {
            return Scenario.ThenAsync(action, a, b);
        }

        protected ThenOptions Then<T1, T2, T3>(Action<T1, T2, T3> action, T1 a, T2 b, T3 c)
        {
            return Scenario.Then(action, a, b, c);
        }

        protected ThenOptions ThenAsync<T1, T2, T3>(Func<T1, T2, T3, Task> action, T1 a, T2 b, T3 c)
        {
            return Scenario.ThenAsync(action, a, b, c);
        }

        protected ThenOptions Then<TStep>(params object[] parameterValues) where TStep : Step
        {
            return Scenario.Then<TStep>(parameterValues);
        }
    }

    class NUnitScenario : ScenarioBase
    {
        public new TException Catch<TException>() where TException : Exception
        {
            return base.Catch<TException>();
        }

        #region Given
        public new GivenOptions Given(Action action)
        {
            return base.Given(action);
        }

        public new GivenOptions GivenAsync(Func<Task> action)
        {
            return base.GivenAsync(action);
        }

        public new GivenOptions Given<T>(Action<T> action, T a)
        {
            return base.Given(action, a);
        }

        public new GivenOptions GivenAsync<T>(Func<T, Task> action, T a)
        {
            return base.GivenAsync(action, a);
        }

        public new GivenOptions Given<T1, T2>(Action<T1, T2> action, T1 a, T2 b)
        {
            return base.Given(action, a, b);
        }

        public new GivenOptions GivenAsync<T1, T2>(Func<T1, T2, Task> action, T1 a, T2 b)
        {
            return base.GivenAsync(action, a, b);
        }

        public new GivenOptions Given<T1, T2, T3>(Action<T1, T2, T3> action, T1 a, T2 b, T3 c)
        {
            return base.Given(action, a, b, c);
        }

        public new GivenOptions GivenAsync<T1, T2, T3>(Func<T1, T2, T3, Task> action, T1 a, T2 b, T3 c)
        {
            return base.GivenAsync(action, a, b, c);
        }

        public new GivenOptions Given<TStep>(params object[] parameterValues) where TStep : Step
        {
            return base.Given<TStep>(parameterValues);
        }
        #endregion

        #region When
        public new WhenOptions When(Action action)
        {
            return base.When(action);
        }

        public new WhenOptions WhenAsync(Func<Task> action)
        {
            return base.WhenAsync(action);
        }

        public new WhenOptions When<T>(Action<T> action, T a)
        {
            return base.When(action, a);
        }

        public new WhenOptions WhenAsync<T>(Func<T, Task> action, T a)
        {
            return base.WhenAsync(action, a);
        }

        public new WhenOptions When<T1, T2>(Action<T1, T2> action, T1 a, T2 b)
        {
            return base.When(action, a, b);
        }

        public new WhenOptions WhenAsync<T1, T2>(Func<T1, T2, Task> action, T1 a, T2 b)
        {
            return base.WhenAsync(action, a, b);
        }

        public new WhenOptions When<T1, T2, T3>(Action<T1, T2, T3> action, T1 a, T2 b, T3 c)
        {
            return base.When(action, a, b, c);
        }

        public new WhenOptions WhenAsync<T1, T2, T3>(Func<T1, T2, T3, Task> action, T1 a, T2 b, T3 c)
        {
            return base.WhenAsync(action, a, b, c);
        }

        public new WhenOptions When<TStep>(params object[] parameterValues) where TStep : Step
        {
            return base.When<TStep>(parameterValues);
        }
        #endregion

        #region Then
        public new ThenOptions Then(Action action)
        {
            return base.Then(action);
        }

        public new ThenOptions ThenAsync(Func<Task> action)
        {
            return base.ThenAsync(action);
        }

        public new ThenOptions Then<T>(Action<T> action, T a)
        {
            return base.Then(action, a);
        }

        public new ThenOptions ThenAsync<T>(Func<T, Task> action, T a)
        {
            return base.ThenAsync(action, a);
        }

        public new ThenOptions Then<T1, T2>(Action<T1, T2> action, T1 a, T2 b)
        {
            return base.Then(action, a, b);
        }

        public new ThenOptions ThenAsync<T1, T2>(Func<T1, T2, Task> action, T1 a, T2 b)
        {
            return base.ThenAsync(action, a, b);
        }

        public new ThenOptions Then<T1, T2, T3>(Action<T1, T2, T3> action, T1 a, T2 b, T3 c)
        {
            return base.Then(action, a, b, c);
        }

        public new ThenOptions ThenAsync<T1, T2, T3>(Func<T1, T2, T3, Task> action, T1 a, T2 b, T3 c)
        {
            return base.ThenAsync(action, a, b, c);
        }

        public new ThenOptions Then<TStep>(params object[] parameterValues) where TStep : Step
        {
            return base.Then<TStep>(parameterValues);
        }
        #endregion
    }
}
