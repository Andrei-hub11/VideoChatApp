namespace VideoChatApp.Contracts.DapperModels;

public class RoomMapping
{
    public Guid RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public IEnumerable<MemberMapping> Members { get; set; } = [];
}
