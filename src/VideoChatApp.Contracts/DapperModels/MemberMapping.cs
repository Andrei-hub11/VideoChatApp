namespace VideoChatApp.Contracts.DapperModels;

public class MemberMapping
{
    public Guid MemberId { get; set; }
    public Guid MemberRoomId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
