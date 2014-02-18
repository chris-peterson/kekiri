using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kekiri.Exceptions;

namespace Kekiri.Impl
{
    internal class LibraryStepIndex
    {
        private static readonly ConcurrentDictionary<string,LibraryStepIndex> _assemblyIndexes = new ConcurrentDictionary<string, LibraryStepIndex>(StringComparer.OrdinalIgnoreCase);
        public static LibraryStepIndex ForAssembly(Assembly assembly)
        {
            return _assemblyIndexes.GetOrAdd(assembly.ShortName(), assemblyShortName =>
            {
                var stepMethods = assembly.GetTypes()
                    .Where(t => typeof (StepLibrary).IsAssignableFrom(t) && !t.IsAbstract)
                    .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                    .Where(m => m.HasAttribute<IStepAttribute>())
                    .Select(m => new LibraryStep(m));

                return new LibraryStepIndex(stepMethods);
            });
        }

        private readonly IStep[] _stepMethods; 

        private LibraryStepIndex(IEnumerable<IStep> stepMethods)
        {
            _stepMethods = stepMethods.ToArray();
            EnsureNoDuplicateStepDefinitions();
        }


        public IStep FindStep(StepType type, string name)
        {
            var step = _stepMethods.SingleOrDefault(m => m.Type == type && m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if(step == null)
                throw new StepNotFound(type, name);

            return step;
        }

        private void EnsureNoDuplicateStepDefinitions()
        {
            var duplicates = _stepMethods.GroupBy(m => m.Name)
                .Where(grouping => grouping.Count() > 1);

            if(duplicates.Any())
                throw new DuplicateStepInStepLibrary(duplicates.First());
        }
    }
}