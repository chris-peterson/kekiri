using System;
using Kekiri.IoC;

namespace Kekiri
{
    public abstract class Step
    {
        public static Step InstanceFor(IContextContainer test, Type stepClass)
        {
            var instance = (Step)Activator.CreateInstance(stepClass);
            instance.SetScenario(test);

            return instance;
        }
        
        private IContextContainer _scenario;

        protected dynamic Context
        {
            get { return _scenario.Context; }
        }

        protected Container Container
        {
            get
            {
                var iocScenario = _scenario as IoCScenarioTest;
                if(iocScenario == null)
                    throw new InvalidOperationException("The Container property requires your scenario to inherit from an IoCScenarioTest");
                return iocScenario.Container;
            }
        }

        private void SetScenario(IContextContainer test)
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