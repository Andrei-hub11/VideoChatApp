using VideoChatApp.Domain.Entities;

namespace VideoChatApp.Application.Contracts.Repositories;

public interface IUserRepository: IRepository
{
    Task<User> GetUserById(string id);
    Task<bool> CreateApplicationUser(User user);
}
