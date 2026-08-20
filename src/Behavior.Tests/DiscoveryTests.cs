using System.Linq;
using System.Threading.Tasks;

namespace Behavior.Tests;

public class DiscoveryTests
{
    [Test]
    public async Task Every_scenario_and_example_row_is_found()
    {
        var titles = Discovered.InFeature("Arithmetic").Select(s => s.Title).ToArray();

        await Assert.That(titles.Length).IsEqualTo(4);
        await Assert.That(titles[0]).IsEqualTo("Adding 1 and 2");
        await Assert.That(titles[1]).IsEqualTo("Adding any two numbers [1, 2, 3]");
        await Assert.That(titles[2]).IsEqualTo("Adding any two numbers [2, 3, 5]");
        await Assert.That(titles[3]).IsEqualTo("Carries its tags");
    }

    [Test]
    public async Task The_feature_is_the_containing_namespace()
    {
        var scenario = Discovered.Named("Adding 1 and 2");

        await Assert.That(scenario.FeatureName).IsEqualTo("Arithmetic");
        await Assert.That(scenario.FeatureProperty.Value).IsEqualTo("Arithmetic");
    }

    [Test]
    public async Task An_example_row_gets_its_own_uid()
    {
        var rows = Discovered.InFeature("Arithmetic")
            .Where(s => s.ScenarioName == "Adding any two numbers")
            .Select(s => s.Uid)
            .ToArray();

        await Assert.That(rows[0].EndsWith(".Adding_any_two_numbers#0")).IsTrue();
        await Assert.That(rows[1].EndsWith(".Adding_any_two_numbers#1")).IsTrue();
    }

    [Test]
    public async Task Tags_come_from_the_method_then_the_fixture()
    {
        var tags = Discovered.Named("Carries its tags").Tags
            .Select(t => $"{t.Key}={t.Value}")
            .ToArray();

        await Assert.That(tags.Length).IsEqualTo(3);
        await Assert.That(tags[0]).IsEqualTo("Category=smoke");
        await Assert.That(tags[1]).IsEqualTo("Category=fast");
        await Assert.That(tags[2]).IsEqualTo("Owner=payments");
    }

    [Test]
    public async Task A_method_without_the_attribute_is_not_a_scenario()
    {
        var steps = Discovered.All().Any(s => s.ScenarioName == "Nothing");

        await Assert.That(steps).IsFalse();
    }
}
