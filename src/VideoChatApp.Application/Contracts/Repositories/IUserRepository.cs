﻿using VideoChatApp.Contracts.DapperModels;
using VideoChatApp.Domain.Entities;

namespace VideoChatApp.Application.Contracts.Repositories;

public interface IUserRepository : IRepository
{
    Task<ApplicationUserMapping?> GetUserByIdAsync(
        string userId,
        CancellationToken cancellationToken
    );
    Task<ApplicationUserMapping?> GetUserByEmailAsync(
        string userEmail,
        CancellationToken cancellationToken
    );
    Task<bool> CreateApplicationUser(User user, CancellationToken cancellationToken);
    Task AddRolesToUser(
        string userId,
        IReadOnlySet<string> roles,
        CancellationToken cancellationToken
    );
    Task<bool> UpdateApplicationUser(User user, CancellationToken cancellationToken);
    Task<IEnumerable<ApplicationUserMapping>> GetTestUsersAsync(
        CancellationToken cancellationToken
    );
    Task DeleteUserAsync(string userId, CancellationToken cancellationToken);
}
