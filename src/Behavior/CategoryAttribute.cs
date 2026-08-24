using System;

namespace Behavior;

/// <summary>
/// A tag under the name Gherkin's @tags conventionally map to in .NET runners, so
/// [Category("fast")] is [Tag("Category", "fast")]. NUnit and MSTest spell theirs the same way.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class CategoryAttribute : TagAttribute
{
    public CategoryAttribute(string value)
        : base("Category", value)
    {
    }
}
