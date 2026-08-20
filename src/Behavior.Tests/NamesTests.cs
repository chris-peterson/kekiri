using System.Threading.Tasks;
using Behavior.Internal;

namespace Behavior.Tests;

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
    public async Task An_acronym_keeps_its_capitals()
    {
        await Assert.That(Names.Sentence("HTTPRequest_is_sent")).IsEqualTo("HTTP request is sent");
    }
}
