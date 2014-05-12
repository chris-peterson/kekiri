using System;
using System.Collections.Generic;
using System.Linq;
using Kekiri.Impl;
using Kekiri.IoC;

namespace Kekiri
{
    public abstract class Step
    {
        internal static Step InstanceFor(IContextAccessor test, Type stepClass, KeyValuePair<string,object>[] parameters)
        {
            var instance = (Step)Activator.CreateInstance(stepClass, parameters.Select(p => p.Value).ToArray());
            instance.SetScenario(test);

            return instance;
        }
        
        private IContextAccessor _scenario;

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

        private void SetScenario(IContextAccessor test)
        {
            _scenario = test;
        }

        public abstract void Execute();
    }

    public abstract class Step<TContext> : Step
    {
        protected new TContext Context
        {
            get { return (TContext)base.Context; }
        }
    }
}