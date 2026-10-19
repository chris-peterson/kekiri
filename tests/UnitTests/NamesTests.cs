using System.Threading.Tasks;
using Kekiri.Internal;

namespace Kekiri.Tests;

public class NamesTests
{
    [Test]
    public async Task Underscores_separate_words()
    {
        await Assert.That(Names.Sentence("Adding_two_numbers")).IsEqualTo("Adding two numbers");
    }

    [Test]
    public async Task Pascal_case_separates_words()
    {
        await Assert.That(Names.Sentence("AddingTwoNumbers")).IsEqualTo("Adding two numbers");
    }

    [Test]
    public async Task Digits_are_words_of_their_own()
    {
        await Assert.That(Names.Sentence("Adding_50_and_70")).IsEqualTo("Adding 50 and 70");
    }

    [Test]
    public async Task Digits_after_a_pascal_case_word_are_a_word_of_their_own()
    {
        await Assert.That(Names.Sentence("Adding50And70")).IsEqualTo("Adding 50 and 70");
    }

    /// <summary>OPERAND1 is a step placeholder and HTTP2 a name; splitting either loses it.</summary>
    [Test]
    public async Task Digits_stay_with_the_capitals_they_follow()
    {
        await Assert.That(Names.Sentence("the_user_enters_OPERAND1")).IsEqualTo("The user enters OPERAND1");
        await Assert.That(Names.Sentence("Uses_HTTP2")).IsEqualTo("Uses HTTP2");
        await Assert.That(Names.Sentence("Step_A1")).IsEqualTo("Step a 1");
    }

    [Test]
    public async Task Nothing_reads_as_nothing()
    {
        await Assert.That(Names.Sentence("")).IsEqualTo("");
        await Assert.That(Names.Sentence(null)).IsNull();
    }

    [Test]
    public async Task An_acronym_keeps_its_capitals()
    {
        await Assert.That(Names.Sentence("HTTPRequest_is_sent")).IsEqualTo("HTTP request is sent");
    }
}
