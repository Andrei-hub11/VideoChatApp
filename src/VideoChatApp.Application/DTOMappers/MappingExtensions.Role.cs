using VideoChatApp.Contracts.Models;
using VideoChatApp.Contracts.Response;

namespace VideoChatApp.Application.DTOMappers;

public static class MappingExtensionsRole
{
    public static RoleResponseDTO ToDTO(this RoleMappingDTO role)
    {
        return new RoleResponseDTO(role.Name);
    }

    public static IReadOnlySet<RoleResponseDTO> ToDTO(this IReadOnlySet<RoleMappingDTO> roles)
    {
        return roles.Select(role => role.ToDTO()).ToHashSet();
    }

    public static IReadOnlySet<string> ToHashSetString(this IReadOnlySet<RoleResponseDTO> roles)
    {
        return roles.Select(role => role.Name).ToHashSet();
    }
}
