using LocalNetAppChat.Domain.Shared;
using NUnit.Framework;

namespace LocalNetAppChat.Domain.Tests.Shared;

[TestFixture]
public class ResultTests
{
    [Test]
    public void When_Result_is_successful_but_without_value_an_exception_is_thrown()
    {
        Assert.Throws<ArgumentNullException>(
            () => Result<string>.Success(null)
            );
    }

    [Test]
    public void When_Result_is_not_Successful_the_error_is_not_empty()
    {
        Assert.Throws<ArgumentException>(
            () => Result<string>.Failure(null)
        );
    }
}