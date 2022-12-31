using System.Diagnostics.CodeAnalysis;

namespace LocalNetAppChat.Server.Domain.Messaging;

/*
 * The DateTimeProvider solemnly exists to enable
 * testability.
 */
[ExcludeFromCodeCoverage]
public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
}