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
        public void TestStartsWithMultipleUppercaseLetters()
        {
            "ABC".StartsWithMultipleUppercaseLetters().Should().BeTrue();
            "abc".StartsWithMultipleUppercaseLetters().Should().BeFalse();
            "AbC".StartsWithMultipleUppercaseLetters().Should().BeFalse();
            " ABC".StartsWithMultipleUppercaseLetters().Should().BeTrue();
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