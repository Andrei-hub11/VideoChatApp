using VideoChatApp.Contracts.DapperModels;
using VideoChatApp.Contracts.Models;
using VideoChatApp.Contracts.Response;
using VideoChatApp.Domain.Entities;

namespace VideoChatApp.Application.DTOMappers;

public static class MappingExtensionsUser
{
    public static UserResponseDTO ToDTO(this UserMapping user)
    {
        return new UserResponseDTO(user.Id, user.UserName, user.Email, user.ProfileImagePath);
    }

    public static UserResponseDTO ToDTO(this User user)
    {
        return new UserResponseDTO(user.Id, user.UserName, user.Email, user.ProfileImagePath);
    }

    public static IReadOnlyList<UserResponseDTO> ToDTO(this IEnumerable<UserMapping> users)
    {
        return users.Select(user => user.ToDTO()).ToList();
    }
}
