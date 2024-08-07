namespace VideoChatApp.Contracts.Request;

public sealed record UpdateUserRequestDTO(
    string UserName, 
    string ProfileImage);
