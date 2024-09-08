namespace VideoChatApp.Application.Contracts.Contexts;

public interface IUserContext
{
    bool IsAuthenticated { get; }
    string UserId { get; }
}
