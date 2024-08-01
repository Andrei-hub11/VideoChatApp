using VideoChatApp.Contracts.Models;
using VideoChatApp.Contracts.Response;

namespace VideoChatApp.Application.DTOMappers;

public static class MappingExtensionsRole
{
    public static RoleResponseDTO ToDTO(this RoleMappingDTO role)
    {
        return new RoleResponseDTO(role.Id, role.Name);
    }

    public static HashSet<RoleResponseDTO> ToDTO(this HashSet<RoleMappingDTO> roles)
    {
        return roles.Select(role => role.ToDTO()).ToHashSet();
    }

    public static HashSet<string> ToHashSetString(this HashSet<RoleResponseDTO> roles)
    {
        return roles.Select(role => role.Name).ToHashSet();
    }
}
