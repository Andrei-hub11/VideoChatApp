using VideoChatApp.Contracts.Models;
using VideoChatApp.Contracts.Response;
using VideoChatApp.Domain.Entities;

namespace VideoChatApp.Application.DTOMappers;

public static class MappingExtensionsUser
{
    public static UserResponseDTO ToResponseDTO(this UserMapping user)
    {
        return new UserResponseDTO(user.Id, user.UserName, user.Email, user.ProfileImagePath);
    }

    public static UserResponseDTO ToResponseDTO(this User user)
    {
        return new UserResponseDTO(user.Id, user.UserName, user.Email, user.ProfileImagePath);
    }

    public static UserResponseDTO ToResponseDTO(this UserInfoMapping user)
    {
        return new UserResponseDTO(user.Id, user.UserName, user.Email, user.ProfileImagePath);
    }

    public static IReadOnlyList<UserResponseDTO> ToReponseDTO(this IEnumerable<UserMapping> users)
    {
        return users.Select(user => user.ToResponseDTO()).ToList();
    }
}
