using System;
using System.Collections.Concurrent;

namespace Kekiri
{
    public class StepLibrary
    {
        private static readonly ConcurrentDictionary<ScenarioTest, StepLibrary> _stepLibraryInstances = new ConcurrentDictionary<ScenarioTest, StepLibrary>();

        public static StepLibrary InstanceFor(ScenarioTest test, Type stepLibraryType)
        {
            return _stepLibraryInstances.GetOrAdd(test, t =>
            {
                var instance = (StepLibrary)Activator.CreateInstance(stepLibraryType);
                instance.SetScenario(test);

                return instance;
            });
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
    }


}