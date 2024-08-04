using VideoChatApp.Contracts.DapperModels;
using VideoChatApp.Domain.Entities;

namespace VideoChatApp.Application.Contracts.Repositories;

public interface IUserRepository: IRepository
{
    Task<User> GetUserById(string id);
    Task<ApplicationUserMapping?> GetUserByEmailAsync(string userEmail, CancellationToken cancellationToken);
    Task<bool> CreateApplicationUser(User user, CancellationToken cancellationToken);
    Task AddRolesToUser(string userId, IReadOnlySet<string> roles, CancellationToken cancellationToken);
}
