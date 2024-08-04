using Dapper;

using System.Data;

using VideoChatApp.Application.Contracts.Repositories;
using VideoChatApp.Contracts.DapperModels;
using VideoChatApp.Domain.Entities;
namespace VideoChatApp.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private IDbConnection? _connection = null;
    private IDbTransaction? _transaction = null;

    private IDbConnection Connection => _connection ?? throw new InvalidOperationException("Connection has not been initialized.");
    private IDbTransaction Transaction => _transaction ?? throw new InvalidOperationException("Transaction has not been initialized.");

    public void Initialize(IDbConnection connection, IDbTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<ApplicationUserMapping?> GetUserByEmailAsync(string userEmail, CancellationToken cancellationToken)
    {
        const string query = @"SELECT u.Id, u.UserName, u.Email, u.PasswordHash, u.ProfileImagePath, ur.Name as RoleName
        FROM ApplicationUsers u
        LEFT JOIN ApplicationUserRoles ur ON u.Id = ur.UserId
        WHERE u.Email = @Email";

        var userDictionary = new Dictionary<string, ApplicationUserMapping>();

        var result = await Connection.QueryAsync<ApplicationUserMapping, string, ApplicationUserMapping>(
            new CommandDefinition(query, new { Email = userEmail }, cancellationToken: cancellationToken, transaction: Transaction),
            (user, role) =>
            {
                if (!userDictionary.TryGetValue(user.Id, out var userEntry))
                {
                    userEntry = user;
                    userEntry.Roles = new HashSet<string>();
                    userDictionary.Add(userEntry.Id, userEntry);
                }

                if (role != null)
                {
                    ((HashSet<string>)userEntry.Roles).Add(role);
                }

                return userEntry;
            },
            splitOn: "RoleName");

        return userDictionary.Values.FirstOrDefault();
    }


    public async Task<bool> CreateApplicationUser(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string query = @"INSERT INTO ApplicationUsers (Id, UserName, Email, PasswordHash, ProfileImage,
        ProfileImagePath) VALUES (@Id, @UserName, @Email, @PasswordHash, @ProfileImage,
        @ProfileImagePath)";

        int result = await Connection.ExecuteAsync(query, new
        {
            user.Id,
            user.UserName,
            user.Email,
            user.PasswordHash,
            user.ProfileImage,
            user.ProfileImagePath
        }, transaction: Transaction);

        return result > 0;
    }

    public async Task AddRolesToUser(string userId, IReadOnlySet<string> roles, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string query = @"INSERT INTO ApplicationUserRoles (UserId, Name) VALUES (@UserId, @Name)";

        foreach (var item in roles)
        {
            await Connection.ExecuteAsync(query, new
            {
                UserId = userId,
                Name = item,
            }, transaction: Transaction);
        }
    }

    public Task<User> GetUserById(string id)
    {
        throw new NotImplementedException();
    }
}
