using System.Linq;
using System.Threading.Tasks;
using Kekiri.Mtp;
using Kekiri.Mtp.Internal;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Requests;

namespace Kekiri.Tests;

public class FilterTests
{
    [Test]
    public async Task A_uid_filter_selects_one_scenario()
    {
        var wanted = Discovered.Named("Adding 1 and 2");
        var filter = new TestNodeUidListFilter(new[] { new TestNodeUid(wanted.Uid) });

        var selected = ScenarioFilter.Apply(filter, Discovered.All()).ToArray();

        await Assert.That(selected.Length).IsEqualTo(1);
        await Assert.That(selected[0].Title).IsEqualTo("Adding 1 and 2");
    }

    [Test]
    public async Task A_uid_filter_that_matches_nothing_selects_nothing()
    {
        var filter = new TestNodeUidListFilter(new[] { new TestNodeUid("no.such.scenario") });

        await Assert.That(ScenarioFilter.Apply(filter, Discovered.All()).Count()).IsEqualTo(0);
    }

    [Test]
    public async Task An_unrecognized_filter_runs_everything()
    {
        var all = Discovered.All();

        await Assert.That(ScenarioFilter.Apply(new NopFilter(), all).Count()).IsEqualTo(all.Count);
    }

    [Test]
    [Arguments("/**[Category=smoke]", "Carries its tags")]
    [Arguments("/**[Feature=Typed]", "The context carries state between steps")]
    [Arguments("/*/*/Adding_numbers/Adding_1_and_2", "Adding 1 and 2")]
    public async Task A_tree_filter_selects_by_tag_feature_or_path(string filter, string title)
    {
        var selected = ScenarioFilter.Apply(Platform.TreeFilter(filter), Discovered.All()).ToArray();

        await Assert.That(selected.Select(s => s.Title).ToArray()).IsEquivalentTo(new[] { title });
    }

    /// <summary>
    /// The platform builds a TreeNodeFilter from --treenode-filter and resolves its [name=value]
    /// expressions against this bag, so what a filter can name is what the bag carries.
    /// </summary>
    [Test]
    public async Task The_feature_and_the_tags_are_what_a_filter_resolves_against()
    {
        var properties = Discovered.Named("Carries its tags").FilterProperties
            .OfType<TestMetadataProperty>()
            .Select(p => $"{p.Key}={p.Value}")
            .ToArray();

        await Assert.That(properties.Contains("Feature=Arithmetic")).IsTrue();
        await Assert.That(properties.Contains("Category=fast")).IsTrue();
        await Assert.That(properties.Contains("Category=smoke")).IsTrue();
        await Assert.That(properties.Contains("Owner=payments")).IsTrue();
    }
}
