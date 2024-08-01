using System.Data;

namespace VideoChatApp.Application.Contracts.Repositories;

public interface IRepository
{
    void Initialize(IDbConnection connection, IDbTransaction transaction);
}

