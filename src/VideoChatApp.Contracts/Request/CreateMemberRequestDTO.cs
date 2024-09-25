namespace VideoChatApp.Contracts.Request;

public sealed record CreateMemberRequestDTO(
    string UserId, 
    string MemberName, 
    string Role
    );
