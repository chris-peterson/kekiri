using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kekiri.Mtp;

namespace Kekiri.Tests.Docs;

/// <summary>
/// The backstop for the C# in the docs: every snippet compiles against the packages as they are
/// now, and every scenario in one runs and passes. A rename, a removed overload, or an example
/// that no longer does what its prose says fails here rather than in a reader's project.
/// </summary>
public class DocSnippetTests
{
    public static IEnumerable<Snippet> Checked() => Snippets.All().Where(s => s.Marker != "skip");

    [Test]
    [MethodDataSource(nameof(Checked))]
    public async Task A_docs_snippet_compiles(Snippet snippet)
    {
        var result = SnippetCompiler.Compile(snippet);

        await Assert.That(string.Join("\n", result.Errors)).IsEqualTo(string.Empty);
    }

    [Test]
    [MethodDataSource(nameof(Checked))]
    public async Task The_scenarios_in_a_docs_snippet_pass(Snippet snippet)
    {
        if (snippet.Marker == "compile-only")
        {
            return;
        }

        var result = SnippetCompiler.Compile(snippet);
        if (result.Assembly == null)
        {
            return;
        }

        foreach (var scenario in Scenario.DiscoverIn(result.Assembly))
        {
            var outcome = await scenario.RunAsync();

            await Assert.That(outcome.Failure).IsNull().Because($"{scenario.Title} in {snippet}");
        }
    }

    [Test]
    public async Task Every_marker_is_one_the_backstop_understands()
    {
        var unknown = Snippets.All()
            .Where(s => s.Marker != null && s.Marker != "skip" && s.Marker != "compile-only")
            .Select(s => $"{s}: {s.Marker}")
            .ToArray();

        await Assert.That(unknown).IsEmpty();
    }

    [Test]
    public async Task The_docs_have_snippets_to_check()
    {
        await Assert.That(Checked().Count()).IsGreaterThan(0);
    }
}
