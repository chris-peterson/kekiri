using System;
using System.Collections.Generic;
using System.Linq;
using Kekiri.Impl;
using Kekiri.IoC;

namespace Kekiri
{
    public abstract class Step
    {
        internal static Step InstanceFor(IContextAccessor test, Type stepClass, KeyValuePair<string, object>[] parameters, IRunnerCatchException runnerCatchException)
        {
            var instance = (Step)Activator.CreateInstance(stepClass, parameters.Select(p => p.Value).ToArray());
            instance.SetScenario(test, runnerCatchException);

            return instance;
        }
        
        private IContextAccessor _scenario;
        private IRunnerCatchException _runnerCatchException;

        protected dynamic Context
        {
            get { return _scenario.Context; }
        }

        protected Container Container
        {
            get
            {
                var iocScenario = _scenario as IContainerAccessor;
                if(iocScenario == null)
                    throw new InvalidOperationException("The Container property requires your scenario to inherit from an IoCScenarioTest");
                return iocScenario.Container;
            }
        }

        private void SetScenario(IContextAccessor test, IRunnerCatchException runnerCatchException)
        {
            _scenario = test;
            _runnerCatchException = runnerCatchException;
        }

        public abstract void Execute();

        public TException Catch<TException>() where TException : Exception
        {
            return _runnerCatchException.Catch<TException>();
        }
    }

    public abstract class Step<TContext> : Step
    {
        protected new TContext Context
        {
            get { return (TContext)base.Context; }
        }
    }
}