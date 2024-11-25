using System.Data;
using Dapper;
using VideoChatApp.Application.Contracts.Repositories;
using VideoChatApp.Contracts.DapperModels;
using VideoChatApp.Domain.Entities;

namespace VideoChatApp.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private IDbConnection? _connection = null;
    private IDbTransaction? _transaction = null;

    private IDbConnection Connection =>
        _connection ?? throw new InvalidOperationException("Connection has not been initialized.");
    private IDbTransaction Transaction =>
        _transaction
        ?? throw new InvalidOperationException("Transaction has not been initialized.");

    public void Initialize(IDbConnection connection, IDbTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<ApplicationUserMapping?> GetUserByIdAsync(
        string userId,
        CancellationToken cancellationToken
    )
    {
        const string query =
            @"SELECT u.Id, u.UserName, u.Email, u.ProfileImage,
        u.ProfileImagePath, ur.Name as RoleName
        FROM ApplicationUsers u
        LEFT JOIN ApplicationUserRoles ur ON u.Id = ur.UserId
        WHERE u.Id = @Id";

        var userDictionary = new Dictionary<string, ApplicationUserMapping>();

        var result = await Connection.QueryAsync<
            ApplicationUserMapping,
            string,
            ApplicationUserMapping
        >(
            new CommandDefinition(
                query,
                new { Id = userId },
                cancellationToken: cancellationToken,
                transaction: Transaction
            ),
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
            splitOn: "RoleName"
        );

        return userDictionary.Values.FirstOrDefault();
    }

    public async Task<ApplicationUserMapping?> GetUserByEmailAsync(
        string userEmail,
        CancellationToken cancellationToken
    )
    {
        const string query =
            @"SELECT u.Id, u.UserName, u.Email, u.ProfileImage,
        u.ProfileImagePath, ur.Name as RoleName
        FROM ApplicationUsers u
        LEFT JOIN ApplicationUserRoles ur ON u.Id = ur.UserId
        WHERE u.Email = @Email";

        var userDictionary = new Dictionary<string, ApplicationUserMapping>();

        var result = await Connection.QueryAsync<
            ApplicationUserMapping,
            string,
            ApplicationUserMapping
        >(
            new CommandDefinition(
                query,
                new { Email = userEmail },
                cancellationToken: cancellationToken,
                transaction: Transaction
            ),
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
            splitOn: "RoleName"
        );

        return userDictionary.Values.FirstOrDefault();
    }

    public async Task<IEnumerable<ApplicationUserMapping>> GetTestUsersAsync(
    CancellationToken cancellationToken
)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string query =
            @"
            SELECT u.Id, u.UserName, u.Email, u.ProfileImage, u.ProfileImagePath, ur.Name as RoleName
            FROM ApplicationUsers u
            LEFT JOIN ApplicationUserRoles ur ON u.Id = ur.UserId
            WHERE u.Email LIKE '%@test.com' OR u.Email LIKE '%@example.com'";

        var userDictionary = new Dictionary<string, ApplicationUserMapping>();

        await Connection.QueryAsync<ApplicationUserMapping, string, ApplicationUserMapping>(
            query,
            (user, roleName) =>
            {
                if (!userDictionary.TryGetValue(user.Id, out var existingUser))
                {
                    existingUser = user;
                    existingUser.Roles = new HashSet<string>();
                    userDictionary.Add(user.Id, existingUser);
                }

                if (roleName != null)
                {
                    ((HashSet<string>)existingUser.Roles).Add(roleName);
                }

                return existingUser;
            },
            splitOn: "RoleName",
            transaction: Transaction
        );

        return userDictionary.Values;
    }

    public async Task<bool> CreateApplicationUser(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string query =
            @"INSERT INTO ApplicationUsers (Id, UserName, Email, ProfileImage,
        ProfileImagePath) VALUES (@Id, @UserName, @Email, @ProfileImage,
        @ProfileImagePath)";

        int result = await Connection.ExecuteAsync(
            query,
            new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.ProfileImage,
                user.ProfileImagePath,
            },
            transaction: Transaction
        );

        return result > 0;
    }

    public async Task AddRolesToUser(
        string userId,
        IReadOnlySet<string> roles,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string query =
            @"INSERT INTO ApplicationUserRoles (UserId, Name) VALUES (@UserId, @Name)";

        foreach (var item in roles)
        {
            await Connection.ExecuteAsync(
                query,
                new { UserId = userId, Name = item },
                transaction: Transaction
            );
        }
    }

    public async Task<bool> UpdateApplicationUser(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string query =
            @"UPDATE ApplicationUsers SET UserName = @UserName, Email = @Email,
        ProfileImage = @ProfileImage, ProfileImagePath = @ProfileImagePath
        WHERE Id = @Id";

        int result = await Connection.ExecuteAsync(
            query,
            new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.ProfileImage,
                user.ProfileImagePath,
            },
            transaction: Transaction
        );

        return result > 0;
    }

    public async Task DeleteUserAsync(string userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string deleteRolesQuery = "DELETE FROM ApplicationUserRoles WHERE UserId = @UserId";
        const string deleteUserQuery = "DELETE FROM ApplicationUsers WHERE Id = @UserId";

        await Connection.ExecuteAsync(
            deleteRolesQuery,
            new { UserId = userId },
            transaction: Transaction
        );
        await Connection.ExecuteAsync(
            deleteUserQuery,
            new { UserId = userId },
            transaction: Transaction
        );
    }
}
