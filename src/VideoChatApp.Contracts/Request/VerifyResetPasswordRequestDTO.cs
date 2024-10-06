namespace VideoChatApp.Contracts.Request;

public record VerifyResetPasswordRequestDTO(string Token, string Email);
