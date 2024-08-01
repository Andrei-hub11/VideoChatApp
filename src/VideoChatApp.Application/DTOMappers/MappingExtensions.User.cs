using VideoChatApp.Contracts.Models;
using VideoChatApp.Contracts.Response;

namespace VideoChatApp.Application.DTOMappers;

public static class MappingExtensionsUser
{
    public static UserResponseDTO ToDTO(this UserMapping user)
    {
        return new UserResponseDTO(user.Id, user.UserName, user.Email, user.ProfileImageUrl);
    }

    public static IReadOnlyList<UserResponseDTO> ToDTO(this IEnumerable<UserMapping> users)
    {
        return users.Select(user => user.ToDTO()).ToList();
    }
}
