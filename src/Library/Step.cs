using System;

namespace Kekiri
{
    public abstract class Step
    {
        public static Step InstanceFor(ScenarioTest test, Type stepClass)
        {
            var instance = (Step)Activator.CreateInstance(stepClass);
            instance.SetScenario(test);

            return instance;
        }
        
        private ScenarioTest _scenario;

        protected dynamic Context
        {
            get { return _scenario.Context; }
        }

        private void SetScenario(ScenarioTest test)
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