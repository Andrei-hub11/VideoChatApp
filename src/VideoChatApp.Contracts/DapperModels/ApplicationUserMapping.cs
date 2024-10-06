namespace VideoChatApp.Contracts.DapperModels;

public class ApplicationUserMapping
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email {  get; set; } = string.Empty;
    public byte[] ProfileImage { get; set; } = [];
    public string ProfileImagePath { get; set; } = string.Empty;
    public IReadOnlySet<string> Roles { get; set; } = new HashSet<string>();
}
