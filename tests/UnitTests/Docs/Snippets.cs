using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kekiri.Tests.Docs;

/// <summary>One fenced C# block from a Markdown file, and what its marker comment asks of it.</summary>
public sealed class Snippet
{
    public Snippet(string file, int line, string code, string marker)
    {
        File = file;
        Line = line;
        Code = code;
        Marker = marker;
    }

    /// <summary>Repo-relative, so a failure names the file a reader would open.</summary>
    public string File { get; }

    public int Line { get; }

    public string Code { get; }

    /// <summary>
    /// The <c>&lt;!-- snippet: … --&gt;</c> comment on the line above the fence, if any:
    /// <c>skip</c> for a fragment or for code from a previous version, <c>compile-only</c> for a
    /// snippet whose scenarios fail on purpose.
    /// </summary>
    public string Marker { get; }

    public override string ToString() => $"{File}:{Line}";
}

static class Snippets
{
    static readonly Regex Fence = new Regex(@"^\s*```(csharp|c#|cs)\s*$");
    static readonly Regex MarkerComment = new Regex(@"^\s*<!--\s*snippet:\s*([\w-]+)\s*-->\s*$");

    public static string RepoRoot { get; } = FindRepoRoot();

    /// <summary>The Markdown a reader meets: the README, the docs site, and the package READMEs.</summary>
    public static IEnumerable<string> Files() =>
        new[] { Path.Combine(RepoRoot, "README.md") }
            .Concat(Directory.GetFiles(Path.Combine(RepoRoot, "docs"), "*.md"))
            .Concat(Directory.GetFiles(Path.Combine(RepoRoot, "src", "Packages"), "README.md", SearchOption.AllDirectories))
            .OrderBy(f => f, StringComparer.Ordinal);

    public static IEnumerable<Snippet> All() => Files().SelectMany(In);

    public static IEnumerable<Snippet> In(string path)
    {
        var lines = System.IO.File.ReadAllLines(path);
        var relative = Path.GetRelativePath(RepoRoot, path);

        for (var i = 0; i < lines.Length; i++)
        {
            if (!Fence.IsMatch(lines[i]))
            {
                continue;
            }

            var marker = i > 0 && MarkerComment.Match(lines[i - 1]) is { Success: true } m ? m.Groups[1].Value : null;
            var start = i + 1;
            var end = start;
            while (end < lines.Length && !lines[end].TrimStart().StartsWith("```", StringComparison.Ordinal))
            {
                end++;
            }

            yield return new Snippet(relative, start, string.Join("\n", lines[start..end]), marker);
            i = end;
        }
    }

    static string FindRepoRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory != null; directory = directory.Parent)
        {
            if (System.IO.File.Exists(Path.Combine(directory.FullName, "Kekiri.slnx")))
            {
                return directory.FullName;
            }
        }

        throw new InvalidOperationException($"No Kekiri.slnx above {AppContext.BaseDirectory}");
    }
}
