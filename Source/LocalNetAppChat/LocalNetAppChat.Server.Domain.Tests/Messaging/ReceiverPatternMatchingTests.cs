using NUnit.Framework;

namespace LocalNetAppChat.Server.Domain.Tests.Messaging;

[TestFixture]
public class ReceiverPatternMatchingTests
{
    [Test]
    public void The_pattern_does_not_match()
    {
        Assert.False(ReceiverPatternMatcher.DoesMatch("Naseif", "_/"));
        Assert.False(ReceiverPatternMatcher.DoesMatch("Nasei", "Naseif.*"));
        Assert.False(ReceiverPatternMatcher.DoesMatch("Naseif", "build*"));
    }

    [Test]
    public void The_pattern_does_match()
    {
        Assert.True(ReceiverPatternMatcher.DoesMatch("build1", "build*"));
        Assert.True(ReceiverPatternMatcher.DoesMatch("build2", "build*"));
        Assert.True(ReceiverPatternMatcher.DoesMatch("build3", "build*"));
        Assert.True(ReceiverPatternMatcher.DoesMatch("Naseif", "Naseif"));
        Assert.True(ReceiverPatternMatcher.DoesMatch("Naseif", "Naseif*"));
    }
}
