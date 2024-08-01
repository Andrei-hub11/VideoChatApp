using Dapper;
using System.Data;

using VideoChatApp.Application.Contracts.Repositories;
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

    public async Task<bool> CreateApplicationUser(User user)
    {
        const string query = @"INSERT INTO ApplicationUser (Id, UserName, Email, PasswordHash, ProfileImage,
        ProfileImageUrl) VALUES (@Id, @UserName, @Email, @PasswordHash, @ProfileImage,
        @ProfileImageUrl)";

        int result = await Connection.ExecuteAsync(query, new { user.Id, user.UserName, user.Email, user.PasswordHash,
        user.ProfileImage, user.ProfileImageUrl}, transaction: Transaction);

        return result > 0;
    }

    public Task<User> GetUserById(string id)
    {
        throw new NotImplementedException();
    }
}
