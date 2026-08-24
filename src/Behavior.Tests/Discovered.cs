using System.Collections.Generic;
using System.Linq;

namespace Behavior.Tests;

/// <summary>
/// Discovery reads the whole test assembly, so a test that cares about one fixture says which
/// feature it means. The sample fixtures live under Samples, one namespace per feature.
/// </summary>
static class Discovered
{
    public static IReadOnlyList<Scenario> All() =>
        Scenario.DiscoverIn(typeof(Discovered).Assembly).ToArray();

    public static IReadOnlyList<Scenario> InFeature(string feature) =>
        All().Where(s => s.FeatureName == feature).ToArray();

    public static Scenario Named(string title) => All().Single(s => s.Title == title);
}
