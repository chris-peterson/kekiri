using System.Collections.Generic;
using FluentAssertions;
using Kekiri.Impl;
using NUnit.Framework;

namespace Kekiri.UnitTests.Reporting
{
    [TestFixture]
    public class Reporting_helpers
    {
        [Test]
        [TestCase("ABC", true)]
        [TestCase(" ABC", true)]
        [TestCase("abc", false)]
        [TestCase("AbC", false)]
        [TestCase("A String", false)]
        public void TestStartsWithMultipleUppercaseLetters(string str, bool expectation)
        {
            str.StartsWithMultipleUppercaseLetters().Should().Be(expectation);
        }

        [Test]
        [TestCase("PascalNaming", "Pascal naming")]
        [TestCase("Underscore_naming", "Underscore naming")]
        [TestCase("Underscore_PRESERVES_CASING", "Underscore PRESERVES CASING")]
        [TestCase("STARTS_WITH_CAPS", "STARTS WITH CAPS")]
        public void TestWithSpaces(string input, string output)
        {
            input.WithSpaces().Should().Be(output);
        }

        [Test]
        public void TestParameterSubstitution()
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("PARAM", "foo")
            };
            new StepName(StepType.Given, "PARAM", parameters)
                .PrettyName
                .Should().Be("foo");
        }
    }
}