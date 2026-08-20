using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Behavior.Internal.Config;
using Behavior.Internal.Reporting;

namespace Behavior.Internal;

static class StepNameStringHelpers
{
    public static string RemovePrefix(this string stepName, string prefix)
    {
        if (string.IsNullOrEmpty(stepName))
        {
            return null;
        }

        if (stepName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            if (stepName.Length == prefix.Length)
            {
                // there is nothing meaningful to output
                return null;
            }

            return stepName.Substring(prefix.Length);
        }

        return stepName;
    }

    public static string WithFirstLetterLowercase(this string str)
    {
        return string.IsNullOrWhiteSpace(str) ? string.Empty : $"{char.ToLower(str[0])}{(str.Length == 1 ? null : str.Substring(1))}";
    }

    public static bool StartsWithMultipleUppercaseLetters(this string str)
    {
        int uppercaseCount = 0;

        foreach (var c in str.SkipWhile(c => !char.IsLetterOrDigit(c)))
        {
            if (char.IsUpper(c))
            {
                uppercaseCount++;
            }
            else
            {
                break;
            }
        }
        return uppercaseCount > 1;
    }

    public static string ToLowerExceptFirstLetter(this string str)
    {
        return string.IsNullOrWhiteSpace(str) ? string.Empty : $"{str[0]}{(str.Length == 1 ? null : str.Substring(1).ToLower())}";
    }

    public static string AsSentence(this string str)
    {
        bool usingUnderscoreNamingConvention = str.Contains("_");

        if (usingUnderscoreNamingConvention)
        {
            return str.Replace("_", " ").TrimStart();
        }

        // pascal casing -- Adapted from: http://stackoverflow.com/questions/272633/add-spaces-before-capital-letters#272929
        var sentence = Regex.Replace(
            str, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");

        return sentence.StartsWithMultipleUppercaseLetters()
            ? sentence
            : sentence.ToLowerExceptFirstLetter();
    }
}
