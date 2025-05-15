namespace VideoChatApp.Contracts.DapperModels;

public class MessageMapping
{
    public Guid MessageId { get; set; }
    public Guid RoomId { get; set; }
    public Guid MemberId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}
