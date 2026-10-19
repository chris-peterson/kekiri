using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kekiri.Internal;
using Kekiri.Internal.Config;

namespace Kekiri.Tests;

public class StepNameTests
{
    static KeyValuePair<string, object>[] Arguments(params (string Name, object Value)[] arguments)
    {
        var pairs = new KeyValuePair<string, object>[arguments.Length];
        for (var i = 0; i < arguments.Length; i++)
        {
            pairs[i] = new KeyValuePair<string, object>(arguments[i].Name, arguments[i].Value);
        }

        return pairs;
    }

    [Test]
    [Arguments("Given", "a_calculator", "a calculator")]
    [Arguments("Given", "Given_a_calculator", "a calculator")]
    [Arguments("Given", "And_a_calculator", "a calculator")]
    [Arguments("When", "adding", "adding")]
    [Arguments("Then", "the_result_is_120", "the result is 120")]
    [Arguments("Given", "ACalculator", "a calculator")]
    [Arguments("Given", "HTTP_is_up", "HTTP is up")]
    [Arguments("Given", "HTTPIsUp", "HTTP is up")]
    [Arguments("Given", "the_HttpClient_is_ready", "the http client is ready")]
    [Arguments("Given", "GivenTheUserIsLoggedIn", "the user is logged in")]
    [Arguments("When", "Adding50And70", "adding 50 and 70")]
    public async Task A_step_name_reads_as_prose(string type, string method, string expected)
    {
        await Assert.That(new StepName(Enum.Parse<StepType>(type), method).PrettyName).IsEqualTo(expected);
    }

    [Test]
    [Arguments("Given", "Given")]
    [Arguments("When", "When")]
    public async Task A_name_that_is_only_the_keyword_reads_as_nothing(string type, string method)
    {
        await Assert.That(new StepName(Enum.Parse<StepType>(type), method).PrettyName).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task A_then_that_starts_with_but_is_joined_with_but()
    {
        await Assert.That(new StepName(StepType.Then, "But_it_is_not_saved").SeparatorToken)
            .IsEqualTo(TokenType.But);
        await Assert.That(new StepName(StepType.Then, "it_is_saved").SeparatorToken)
            .IsEqualTo(TokenType.And);
        await Assert.That(new StepName(StepType.Given, "But_not_this").SeparatorToken)
            .IsEqualTo(TokenType.And);
    }

    [Test]
    public async Task An_uppercase_word_matching_a_parameter_is_replaced_by_its_value()
    {
        var name = new StepName(StepType.Then, "the_result_is_EXPECTED", Arguments(("expected", 120)));

        await Assert.That(name.PrettyName).IsEqualTo("the result is 120");
    }

    [Test]
    public async Task A_placeholder_can_end_in_digits()
    {
        var name = new StepName(StepType.Given, "the_user_enters_OPERAND1", Arguments(("operand1", 12)));

        await Assert.That(name.PrettyName).IsEqualTo("the user enters 12");
    }

    /// <summary>
    /// A step and the scenario it belongs to are both identifiers written as prose, so they read
    /// by the same rules.
    /// </summary>
    [Test]
    [Arguments("HTTPIsUp")]
    [Arguments("the_HttpClient_is_ready")]
    [Arguments("Adding50And70")]
    [Arguments("a_calculator")]
    public async Task A_step_reads_the_way_a_scenario_of_the_same_name_does(string identifier)
    {
        var step = new StepName(StepType.When, identifier).PrettyName;
        var scenario = Names.Sentence(identifier);

        // A step reads mid-sentence, so it starts lowercase unless it opens with an acronym.
        var expected = scenario.StartsWithMultipleUppercaseLetters() ? scenario : scenario.WithFirstLetterLowercase();

        await Assert.That(step).IsEqualTo(expected);
    }

    [Test]
    public async Task A_parameter_with_no_placeholder_is_appended()
    {
        var name = new StepName(StepType.When, "adding", Arguments(("first", 1), ("second", 2)));

        await Assert.That(name.PrettyName).IsEqualTo("adding 1 2");
    }

    [Test]
    public async Task A_null_argument_reads_as_null()
    {
        await Assert.That(new StepName(StepType.When, "saving", Arguments(("value", null))).PrettyName)
            .IsEqualTo("saving {null}");
        await Assert.That(new StepName(StepType.When, "saving_VALUE", Arguments(("value", null))).PrettyName)
            .IsEqualTo("saving {null}");
    }

    [Test]
    public async Task Two_names_are_equal_when_their_type_and_prose_are()
    {
        var name = new StepName(StepType.Given, "a_calculator");
        object same = new StepName(StepType.Given, "Given_a_calculator");

        await Assert.That(name.Equals(same)).IsTrue();
        await Assert.That(name.Equals(name)).IsTrue();
        await Assert.That(name.GetHashCode()).IsEqualTo(same.GetHashCode());
        await Assert.That(name.Equals(new StepName(StepType.When, "a_calculator"))).IsFalse();
        await Assert.That(name.Equals(null)).IsFalse();
        await Assert.That(name.Equals("a calculator")).IsFalse();
        await Assert.That(name.ToString()).IsEqualTo("Given a calculator");
    }

    [Test]
    public async Task The_string_helpers_tolerate_empty_input()
    {
        await Assert.That(((string)null).RemovePrefix("Given")).IsNull();
        await Assert.That("Given".RemovePrefix("Given")).IsNull();
        await Assert.That(" ".WithFirstLetterLowercase()).IsEqualTo(string.Empty);
        await Assert.That("A".WithFirstLetterLowercase()).IsEqualTo("a");
    }
}
