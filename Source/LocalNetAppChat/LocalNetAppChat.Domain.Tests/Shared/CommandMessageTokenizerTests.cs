using LocalNetAppChat.Domain.Shared;
using NUnit.Framework;

namespace LocalNetAppChat.Domain.Tests.Shared;

[TestFixture]
public class CommandMessageTokenizerTests
{
    [Test]
    public void When_in_the_message_there_is_no_space_left_the_rest_is_the_token()
    {
        var message= "token";
        var token = CommandMessageTokenizer.GetToken(ref message);
        Assert.AreEqual("token", token);
        Assert.AreEqual("", message);
    }

    [Test]
    public void GetToken_extracts_first_token_and_leaves_rest()
    {
        var message = "first second third";
        var token = CommandMessageTokenizer.GetToken(ref message);

        Assert.That(token, Is.EqualTo("first"));
        Assert.That(message, Is.EqualTo("second third"));
    }

    [Test]
    public void GetToken_multiple_calls_extract_all_tokens()
    {
        var message = "one two three";

        var first = CommandMessageTokenizer.GetToken(ref message);
        var second = CommandMessageTokenizer.GetToken(ref message);
        var third = CommandMessageTokenizer.GetToken(ref message);

        Assert.That(first, Is.EqualTo("one"));
        Assert.That(second, Is.EqualTo("two"));
        Assert.That(third, Is.EqualTo("three"));
        Assert.That(message, Is.EqualTo(""));
    }

    [Test]
    public void GetToken_trims_leading_spaces_from_rest()
    {
        var message = "first   second";
        var token = CommandMessageTokenizer.GetToken(ref message);

        Assert.That(token, Is.EqualTo("first"));
        Assert.That(message, Is.EqualTo("second"));
    }

    [Test]
    public void GetToken_empty_string_returns_empty_token()
    {
        var message = "";
        var token = CommandMessageTokenizer.GetToken(ref message);

        Assert.That(token, Is.EqualTo(""));
        Assert.That(message, Is.EqualTo(""));
    }

    [Test]
    public void IsCommandMessage_returns_true_for_slash_prefix()
    {
        Assert.That(CommandMessageTokenizer.IsCommandMessage("/ping"), Is.True);
    }

    [Test]
    public void IsCommandMessage_returns_false_for_regular_message()
    {
        Assert.That(CommandMessageTokenizer.IsCommandMessage("hello"), Is.False);
    }

    [Test]
    public void IsCommandMessage_returns_false_for_empty_string()
    {
        Assert.That(CommandMessageTokenizer.IsCommandMessage(""), Is.False);
    }

    [Test]
    public void MessageWithoutCommandSignal_strips_leading_slash()
    {
        var result = CommandMessageTokenizer.MessageWithoutCommandSignal("/msg Alice hello");

        Assert.That(result, Is.EqualTo("msg Alice hello"));
    }

    [Test]
    public void MessageWithoutCommandSignal_trims_whitespace_after_slash()
    {
        var result = CommandMessageTokenizer.MessageWithoutCommandSignal("/  hello");

        Assert.That(result, Is.EqualTo("hello"));
    }

    [Test]
    public void GetToken_handles_single_space_string()
    {
        var message = " ";
        var token = CommandMessageTokenizer.GetToken(ref message);

        Assert.That(token, Is.EqualTo(""));
        Assert.That(message, Is.EqualTo(""));
    }

    [Test]
    public void IsCommandMessage_returns_true_for_slash_only()
    {
        Assert.That(CommandMessageTokenizer.IsCommandMessage("/"), Is.True);
    }
}