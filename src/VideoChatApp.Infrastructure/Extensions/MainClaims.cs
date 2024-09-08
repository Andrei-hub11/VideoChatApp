using System.Security.Claims;

using VideoChatApp.Domain.Exceptions;

namespace VideoChatApp.Infrastructure.Extensions;

internal static class MainClaimsExtensions
{
    public static string GetUserId(this ClaimsPrincipal? principal)
    {
        Claim? userIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            throw new UnauthorizeUserAccessException("O contexto do usuário não está disponível");
        }

        return userIdClaim.Value;
    }
}

