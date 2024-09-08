using Microsoft.AspNetCore.Http;
using VideoChatApp.Domain.Exceptions;
using VideoChatApp.Application.Contracts.Contexts;
using VideoChatApp.Infrastructure.Extensions;

namespace VideoChatApp.Infrastructure.Contexts;

internal sealed class UserContexts : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContexts(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId =>
        _httpContextAccessor.HttpContext?.User?.GetUserId() ??
        throw new UnauthorizeUserAccessException("O contexto do usuário não está disponível");

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ??
        throw new UnauthorizeUserAccessException("O contexto do usuário não está disponível");
}
