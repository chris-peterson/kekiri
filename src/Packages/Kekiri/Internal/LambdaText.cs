using System.Text.RegularExpressions;

namespace Kekiri.Internal;

/// <summary>
/// What a lambda step reads as. Its method name is the compiler's (<c>&lt;Scenario&gt;b__1_0</c>),
/// so the Gherkin shows the lambda's body as the reader wrote it instead.
/// </summary>
static class LambdaText
{
    public static bool IsCompilerGenerated(string methodName) => methodName.StartsWith("<");

    /// <summary>
    /// The body of <paramref name="expression"/>, on one line: <c>() =&gt; _total += 1</c> reads
    /// <c>_total += 1</c>, and a block body reads as its statements without the braces.
    /// </summary>
    public static string Body(string expression)
    {
        var arrow = expression.IndexOf("=>");
        var body = (arrow < 0 ? expression : expression.Substring(arrow + 2)).Trim();

        if (body.StartsWith("{") && body.EndsWith("}"))
        {
            body = body.Substring(1, body.Length - 2).Trim();
        }

        return Regex.Replace(body, @"\s+", " ");
    }
}
