namespace VideoChatApp.Contracts.Request;

public record UpdatePasswordRequestDTO(string NewPassword, string UserId, string Token);
