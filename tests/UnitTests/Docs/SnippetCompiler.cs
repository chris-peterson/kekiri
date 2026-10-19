using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kekiri.Tests.Docs;

/// <summary>
/// Compiles a docs snippet the way a reader would use it: in a project that references the Kekiri
/// packages, with the SDK's implicit usings. The Kekiri namespaces are imported too, since a
/// snippet showing a fixture leaves its usings out; a snippet showing an entry point has to
/// carry its own, because a reader has to write them.
/// </summary>
static class SnippetCompiler
{
    const string Usings = @"
global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Net.Http;
global using System.Threading;
global using System.Threading.Tasks;
global using Kekiri;
global using Kekiri.IoC;
global using Kekiri.IoC.Autofac;
global using AwesomeAssertions;
";

    /// <summary>
    /// Code the examples are written against but don't show, because it is the code under test
    /// rather than the scenario. Left out when a snippet declares a type of the same name.
    /// </summary>
    static readonly Dictionary<string, string> Support = new Dictionary<string, string>
    {
        ["Calculator"] = @"
class Calculator
{
    public decimal Operand1 { get; set; }
    public decimal Operand2 { get; set; }
    public decimal Result { get; set; }
    public void Add() => Result = Operand1 + Operand2;
    public void Subtract() => Result = Operand1 - Operand2;
    public void Divide() => Result = Operand1 / Operand2;
}",
        ["SomeServiceType"] = "class SomeServiceType { }",
    };

    static readonly Lazy<MetadataReference[]> References = new Lazy<MetadataReference[]>(() =>
        ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))
            .Split(Path.PathSeparator)
            .Where(p => p.Length > 0)
            .Select(p => (MetadataReference)MetadataReference.CreateFromFile(p))
            .ToArray());

    public sealed class Result
    {
        public Result(Assembly assembly, IReadOnlyList<string> errors)
        {
            Assembly = assembly;
            Errors = errors;
        }

        public Assembly Assembly { get; }

        public IReadOnlyList<string> Errors { get; }
    }

    public static Result Compile(Snippet snippet)
    {
        var parse = new CSharpParseOptions(LanguageVersion.Latest);
        var code = CSharpSyntaxTree.ParseText(snippet.Code, parse, path: snippet.File);
        var declared = code.GetRoot().DescendantNodes().OfType<BaseTypeDeclarationSyntax>()
            .Select(t => t.Identifier.Text)
            .ToHashSet();
        var hasEntryPoint = code.GetRoot().ChildNodes().OfType<GlobalStatementSyntax>().Any();

        var trees = new List<SyntaxTree> { code, CSharpSyntaxTree.ParseText(Usings, parse) };
        trees.AddRange(Support
            .Where(s => !declared.Contains(s.Key))
            .Select(s => CSharpSyntaxTree.ParseText(s.Value, parse)));

        var compilation = CSharpCompilation.Create(
            $"Snippet_{Guid.NewGuid():N}",
            trees,
            References.Value,
            new CSharpCompilationOptions(
                hasEntryPoint ? OutputKind.ConsoleApplication : OutputKind.DynamicallyLinkedLibrary,
                nullableContextOptions: NullableContextOptions.Disable));

        using var image = new MemoryStream();
        var emitted = compilation.Emit(image);

        var errors = emitted.Diagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .Select(d => d.ToString())
            .ToArray();

        return new Result(emitted.Success ? System.Reflection.Assembly.Load(image.ToArray()) : null, errors);
    }
}
