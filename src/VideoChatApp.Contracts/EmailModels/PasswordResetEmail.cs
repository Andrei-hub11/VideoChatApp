namespace VideoChatApp.Contracts.EmailModels;

public sealed record PasswordResetEmail(string ResetLink, TimeSpan TokenValidity);
