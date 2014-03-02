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

      protected FluentScenario()
      {
// ReSharper disable DoNotCallOverridableMethodsInConstructor
         var reportTarget = CreateReportTarget();
// ReSharper restore DoNotCallOverridableMethodsInConstructor
         _scenarioRunner = new ScenarioRunner(this, reportTarget);
      }

      [Test]
      public virtual void RunScenario()
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

      public abstract class FluentOptionsForStep
      {
         private readonly FluentScenario _scenario;

         protected FluentOptionsForStep(FluentScenario scenario)
         {
            _scenario = scenario;
         }

         protected abstract StepType StepType { get; }

         #region And
         public FluentOptionsForStep And(Action action)
         {
            _scenario.AddStepMethod(StepType, action);
            return this;
         }

         public FluentOptionsForStep And<T>(Action<T> action, T a)
         {
            _scenario.AddStepMethod(StepType, action.Method, a);
            return this;
         }

         public FluentOptionsForStep And<T1, T2>(Action<T1, T2> action, T1 a, T2 b)
         {
            _scenario.AddStepMethod(StepType, action.Method, a, b);
            return this;
         }

         public FluentOptionsForStep And<T1, T2, T3>(Action<T1, T2, T3> action, T1 a, T2 b, T3 c)
         {
            _scenario.AddStepMethod(StepType, action.Method, a, b, c);
            return this;
         }

         public FluentOptionsForStep And<TStep>(params object[] parameterValues) where TStep : Step
         {
            _scenario.AddStepClass<TStep>(StepType, parameterValues);
            return this;
         }
         #endregion

         #region But
         public FluentOptionsForStep But(Action action)
         {
            _scenario.AddStepMethod(StepType, action);
            return this;
         }

         public FluentOptionsForStep But<T>(Action<T> action, T a)
         {
            _scenario.AddStepMethod(StepType, action.Method, a);
            return this;
         }

         public FluentOptionsForStep But<T1, T2>(Action<T1, T2> action, T1 a, T2 b)
         {
            _scenario.AddStepMethod(StepType, action.Method, a, b);
            return this;
         }

         public FluentOptionsForStep But<T1, T2, T3>(Action<T1, T2, T3> action, T1 a, T2 b, T3 c)
         {
            _scenario.AddStepMethod(StepType, action.Method, a, b, c);
            return this;
         }

         public FluentOptionsForStep But<TStep>(params object[] parameterValues) where TStep : Step
         {
            _scenario.AddStepClass<TStep>(StepType, parameterValues);
            return this;
         }
         #endregion
      }

      #region Given
      public class FluentOptionsForGiven : FluentOptionsForStep
      {
         public FluentOptionsForGiven(FluentScenario scenario) : base(scenario)
         {
         }

         protected override StepType StepType
         {
            get { return StepType.Given; }
         }
      }

      protected FluentOptionsForStep Given(Action action)
      {
         return new FluentOptionsForGiven(this).And(action);
      }

      protected FluentOptionsForStep Given<T>(Action<T> action, T a)
      {
         return new FluentOptionsForGiven(this).And(action, a);
      }

      protected FluentOptionsForStep Given<T1, T2>(Action<T1, T2> action, T1 a, T2 b)
      {
         return new FluentOptionsForGiven(this).And(action, a, b);
      }

      protected FluentOptionsForStep Given<T1, T2, T3>(Action<T1, T2, T3> action, T1 a, T2 b, T3 c)
      {
         return new FluentOptionsForGiven(this).And(action, a, b, c);
      }

      protected FluentOptionsForStep Given<TStep>(params object[] parameterValues) where TStep : Step
      {
         return new FluentOptionsForGiven(this).And<TStep>(parameterValues);
      }

      #endregion

      #region When

      // NOTE: this class is introduced for symmetry and DRY purposes, but we don't currently expose a fluent handle 
      // to the API caller since only a single When is supported.
      // May want to revisit and fork the type heirarchy to have Given/When share a derivation but When not
      public class FluentOptionsForWhen : FluentOptionsForStep
      {
         public FluentOptionsForWhen(FluentScenario scenario) : base(scenario)
         {
         }

         protected override StepType StepType
         {
            get { return StepType.When; }
         }
      }

      protected void When(Action action)
      {
         new FluentOptionsForWhen(this).And(action);
      }

      protected void When<T>(Action<T> action, T a)
      {
         new FluentOptionsForWhen(this).And(action, a);
      }

      protected void When<T1, T2>(Action<T1, T2> action, T1 a, T2 b)
      {
         new FluentOptionsForWhen(this).And(action, a, b);
      }

      protected void When<T1, T2, T3>(Action<T1, T2, T3> action, T1 a, T2 b, T3 c)
      {
         new FluentOptionsForWhen(this).And(action, a, b, c);
      }

      protected void When<TStep>(params object[] parameterValues) where TStep : Step
      {
         new FluentOptionsForWhen(this).And<TStep>(parameterValues);
      }
      #endregion

      #region Then
      public class FluentOptionsForThen : FluentOptionsForStep
      {
         public FluentOptionsForThen(FluentScenario scenario) : base(scenario)
         {
         }

         protected override StepType StepType
         {
            get { return StepType.Then; }
         }
      }

      protected FluentOptionsForStep Then(Action action)
      {
         return new FluentOptionsForThen(this).And(action);
      }

      protected FluentOptionsForStep Then<T>(Action<T> action, T a)
      {
         return new FluentOptionsForThen(this).And(action, a);
      }

      protected FluentOptionsForStep Then<T1, T2>(Action<T1, T2> action, T1 a, T2 b)
      {
         return new FluentOptionsForThen(this).And(action, a, b);
      }

      protected FluentOptionsForStep Then<T1, T2, T3>(Action<T1, T2, T3> action, T1 a, T2 b, T3 c)
      {
         return new FluentOptionsForThen(this).And(action, a, b, c);
      }

      protected FluentOptionsForStep Then<TStep>(params object[] parameterValues) where TStep : Step
      {
         return new FluentOptionsForThen(this).And<TStep>(parameterValues);
      }
      #endregion

      private void AddStepMethod(StepType stepType, Action action)
      {
         _scenarioRunner.AddStep(new StepMethodInvoker(stepType, action.Method));
      }

      private void AddStepMethod(StepType stepType, MethodInfo method, params object[] parameterValues)
      {
         var parameters = ExtractParameters(method, parameterValues);
         _scenarioRunner.AddStep(new StepMethodInvoker(stepType, method, parameters));
      }

      private void AddStepClass<TStep>(StepType stepType, params object[] parameterValues) where TStep : Step
      {
         var stepClass = typeof(TStep);
         var parameters = ExtractParameters(stepClass.GetConstructors().Single(), parameterValues);
         _scenarioRunner.AddStep(new StepClassInvoker(stepType, stepClass, parameters));
      }

      private KeyValuePair<string, object>[] ExtractParameters(MethodBase method, object[] parameterValues)
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

      protected virtual void Before() { }
      protected virtual void After() { }

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