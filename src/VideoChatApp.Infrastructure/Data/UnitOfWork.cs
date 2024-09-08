using System.Data;

using Microsoft.Extensions.DependencyInjection;

using VideoChatApp.Application.Contracts.Data;
using VideoChatApp.Application.Contracts.Repositories;

namespace VideoChatApp.Infrastructure.Data;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly DapperContext _dapperContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly IDbConnection _connection;
    private IDbTransaction _transaction;
    private bool _disposed;

    public UnitOfWork(DapperContext dapperContext, IServiceProvider serviceProvider)
    {
        _dapperContext = dapperContext;
        _serviceProvider = serviceProvider;
        _connection = _dapperContext.CreateConnection();
        _connection.Open();
        _transaction = _connection.BeginTransaction();
    }

    public TRepository GetRepository<TRepository>() where TRepository : class, IRepository
    {
        var repository = _serviceProvider.GetRequiredService<TRepository>();
        repository.Initialize(_connection, _transaction);
        return repository;
    }

    public void Commit()
    {
        try
        {
            _transaction?.Commit();
        }
        catch
        {
            _transaction?.Rollback();
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = _connection.BeginTransaction();
        }
    }

    public void Rollback()
    {
        _transaction?.Rollback();
        _transaction?.Dispose();
        _transaction = _connection.BeginTransaction();
    }

    // protected virtual won't be necessary because it's a sealed class
    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _connection?.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}


