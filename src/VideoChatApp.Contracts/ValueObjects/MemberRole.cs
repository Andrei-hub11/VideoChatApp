namespace VideoChatApp.Contracts.ValueObjects;

public sealed class MemberRole
{
    public string Value { get; private set; }

    private MemberRole(string value)
    {
        Value = value;
    }

    public static readonly MemberRole Member = new MemberRole("member");
    public static readonly MemberRole Admin = new MemberRole("admin");
    public static readonly MemberRole Moderator = new MemberRole("moderator");

    public static bool IsValidRole(string role)
    {
        var validRoles = new[] { "member", "admin", "moderator" };
        return validRoles.Contains(role);
    }
}
