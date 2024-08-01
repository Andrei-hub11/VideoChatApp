namespace VideoChatApp.Domain.Exceptions;

public class UnauthorizeUserAccessException : Exception
{
    public UnauthorizeUserAccessException(string message) : base(message)
    { }
}
