using VideoChatApp.Contracts.DapperModels;

namespace VideoChatApp.Tests.Builders;

public class ApplicationUserMappingBuilder
{
    private string _id = "123";
    private string _userName = "John Doe";
    private string _email = "john.doe@example.com";
    private byte[] _profileImage = Array.Empty<byte>();
    private string _profileImagePath = "path/to/image.jpg";
    private IReadOnlySet<string> _roles = new HashSet<string> { "Admin" };

    public ApplicationUserMappingBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public ApplicationUserMappingBuilder WithUserName(string userName)
    {
        _userName = userName;
        return this;
    }

    public ApplicationUserMappingBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public ApplicationUserMappingBuilder WithProfileImage(byte[] profileImage)
    {
        _profileImage = profileImage;
        return this;
    }

    public ApplicationUserMappingBuilder WithProfileImagePath(string profileImagePath)
    {
        _profileImagePath = profileImagePath;
        return this;
    }

    public ApplicationUserMappingBuilder WithRoles(IReadOnlySet<string> roles)
    {
        _roles = roles;
        return this;
    }

    public ApplicationUserMapping Build()
    {
        return new ApplicationUserMapping
        {
            Id = _id,
            UserName = _userName,
            Email = _email,
            ProfileImage = _profileImage,
            ProfileImagePath = _profileImagePath,
            Roles = _roles
        };
    }
}

