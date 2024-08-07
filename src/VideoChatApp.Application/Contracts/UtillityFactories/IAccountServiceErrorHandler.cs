using VideoChatApp.Contracts.DapperModels;
using VideoChatApp.Contracts.Response;

namespace VideoChatApp.Application.Contracts.UtillityFactories;

public interface IAccountServiceErrorHandler
{
    Task HandleRegistrationFailureAsync(UserResponseDTO user, string? profileImagePath);
    Task HandleUnexpectedRegistrationExceptionAsync(string userEmail, string? profileImagePath);
    Task HandleUnexpectedUpdateExceptionAsync(ApplicationUserMapping user);
}
