using VideoChatApp.Application.Contracts.Repositories;

namespace VideoChatApp.Application.Contracts.Data;

public interface IUnitOfWork : IDisposable
{
    TRepository GetRepository<TRepository>() where TRepository : class, IRepository;
    void Commit();
    void Rollback();
}
