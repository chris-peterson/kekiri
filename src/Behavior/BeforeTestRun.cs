using System;
using System.Linq;
using System.Reflection;

namespace Behavior;

static class BeforeTestRun
{
    public static void InvokeAll(Assembly testAssembly)
    {
        var setups = testAssembly.GetTypes()
            .Where(t => !t.IsAbstract && typeof(IBeforeTestRun).IsAssignableFrom(t))
            .OrderBy(t => t.FullName, StringComparer.Ordinal)
            .Select(t => (IBeforeTestRun)Activator.CreateInstance(t));

        foreach (var setup in setups)
        {
            setup.Setup();
        }
    }
}
