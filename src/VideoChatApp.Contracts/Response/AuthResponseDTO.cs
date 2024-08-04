namespace VideoChatApp.Contracts.Response;

public sealed record AuthResponseDTO(
    UserResponseDTO User, 
    string AccessToken, 
    string RefreshToken, 
    IReadOnlySet<RoleResponseDTO> Roles);