namespace VideoChatApp.Contracts.Request;

public sealed record UserRegisterRequestDTO(
    string UserName, 
    string Email, 
    string Password, 
    string ProfileImageUrl);
