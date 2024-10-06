using VideoChatApp.Domain.Entities;

namespace VideoChatApp.Application.Contracts.TokenJWT;

public interface ITokenService
{
    string GeneratePasswordResetToken(User user);
    bool ValidatePasswordResetToken(string token);
}
