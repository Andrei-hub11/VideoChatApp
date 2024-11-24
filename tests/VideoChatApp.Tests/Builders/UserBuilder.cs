using VideoChatApp.Application.Common.Result;
using VideoChatApp.Domain.Entities;

namespace VideoChatApp.Tests.Builders;
public class UserBuilder
{
    private string _id = Guid.NewGuid().ToString();
    private string _name = "Default Name"; 
    private string _email = "default@example.com"; 
    private byte[] _profileImage = Array.Empty<byte>(); 
    private string _profileImagePath = "/images/default-profile.png"; 
    private IReadOnlySet<string> _roles = new HashSet<string> { "User" };

    public UserBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public UserBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserBuilder WithProfileImage(byte[] profileImage)
    {
        _profileImage = profileImage;
        return this;
    }

    public UserBuilder WithProfileImagePath(string profileImagePath)
    {
        _profileImagePath = profileImagePath;
        return this;
    }

    public UserBuilder WithRoles(IReadOnlySet<string> roles)
    {
        _roles = roles;
        return this;
    }

    public UserBuilder AddRole(string role)
    {
        var newRoles = _roles.ToHashSet();
        newRoles.Add(role);
        _roles = newRoles;
        return this;
    }

    public Result<User> Build()
    {
        return User.Create(_id, _name, _email, _profileImage, _profileImagePath, _roles);
    }
}

