using System;
using System.Collections.Generic;
using System.Text;

namespace Behavior.Internal;

/// <summary>
/// A scenario identifier is written to be read back as prose, so rendering one is a casing
/// problem rather than a lookup. Both conventions a suite mixes are handled: snake_case
/// (Adding_two_numbers) and PascalCase (AddingTwoNumbers) both land on "Adding two numbers".
/// </summary>
static class Names
{
    public static string Sentence(string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            return identifier;
        }

        var words = Split(identifier);

        for (var i = 0; i < words.Count; i++)
        {
            words[i] = i == 0 ? Capitalized(words[i]) : Lowered(words[i]);
        }

        return string.Join(" ", words);
    }

    static List<string> Split(string identifier)
    {
        var words = new List<string>();
        var word = new StringBuilder();

        foreach (var c in identifier)
        {
            if (c == '_')
            {
                Flush(words, word);
                continue;
            }

            if (word.Length > 0)
            {
                var previous = word[word.Length - 1];

                // An acronym keeps its last letter for the word that follows it, so HTTPRequest
                // splits as "HTTP" + "Request" rather than "HTTPR" + "equest".
                if (char.IsLower(c) && word.Length > 1 &&
                    char.IsUpper(previous) && char.IsUpper(word[word.Length - 2]))
                {
                    word.Length -= 1;
                    Flush(words, word);
                    word.Append(previous);
                }
                else if ((char.IsUpper(c) && !char.IsUpper(previous)) ||
                         char.IsDigit(c) != char.IsDigit(previous))
                {
                    Flush(words, word);
                }
            }

            word.Append(c);
        }

        Flush(words, word);

        return words;
    }

    static void Flush(List<string> words, StringBuilder word)
    {
        if (word.Length > 0)
        {
            words.Add(word.ToString());
            word.Length = 0;
        }
    }

    static string Capitalized(string word) =>
        IsAcronym(word) ? word : char.ToUpperInvariant(word[0]) + word.Substring(1);

    static string Lowered(string word) =>
        IsAcronym(word) ? word : word.ToLowerInvariant();

    /// <summary>An all-capitals run is a name in its own right; lowercasing it loses the name.</summary>
    static bool IsAcronym(string word)
    {
        if (word.Length < 2)
        {
            return false;
        }

        foreach (var c in word)
        {
            if (char.IsLower(c))
            {
                return false;
            }
        }

        return true;
    }
}
