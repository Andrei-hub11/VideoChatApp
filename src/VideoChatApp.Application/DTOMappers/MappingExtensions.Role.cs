using VideoChatApp.Contracts.Models;

namespace VideoChatApp.Application.DTOMappers;

public static class MappingExtensionsRole
{
    //public static RoleResponseDTO ToDTO(this RoleMappingDTO role)
    //{
    //    return new RoleResponseDTO(role.Name);
    //}

    public static IReadOnlySet<string> ToResponseDTO(this IReadOnlySet<RoleMappingDTO> roles)
    {
        return roles.Select(role => role.Name).ToHashSet();
    }
}
