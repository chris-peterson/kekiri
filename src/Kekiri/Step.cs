using System;
using System.Collections.Generic;
using System.Linq;
using Kekiri.Impl;

namespace Kekiri
{
    public abstract class Step
    {
        internal static Step InstanceFor(ScenarioBase scenario, Type stepClass, KeyValuePair<string, object>[] parameters, IExceptionHandler exceptionHandler)
        {
            var instance = (Step)Activator.CreateInstance(stepClass, parameters.Select(p => p.Value).ToArray());
            instance.SetScenario(scenario, exceptionHandler);

            return instance;
        }

        ScenarioBase _scenario;
        IExceptionHandler _exceptionHandler;

        protected dynamic Context => _scenario.Context;

        void SetScenario(ScenarioBase scenario, IExceptionHandler exceptionHandler)
        {
            _scenario = scenario;
            _exceptionHandler = exceptionHandler;
        }

        public abstract void Execute();

        public TException Catch<TException>() where TException : Exception
        {
            return _exceptionHandler.Catch<TException>();
        }
    }

    public abstract class Step<TContext> : Step
    {
        protected new TContext Context => (TContext)base.Context;
    }
}