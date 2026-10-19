using System;
using System.Collections.Generic;
using System.Linq;
using Kekiri.Internal.Config;
using Kekiri.Internal.Reporting;

namespace Kekiri.Internal;

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
}
