using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using TagNameMapper.Models.EFCore.DbContexts;

namespace TagNameMapper.Models.EFCore.Repositories;

/// <summary>
/// 工作单元实现，管理多个 Repository 的事务
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly TagNameMapperDbContext _context;
    private IDbContextTransaction? _transaction;
    
    // 从依赖注入容器获取的Repository实例
    private readonly ITagRepository _tagRepository;
    private readonly ITagGroupRepository _tagGroupRepository;

    public UnitOfWork(TagNameMapperDbContext context,ITagRepository tagRepository,ITagGroupRepository tagGroupRepository)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
        _tagGroupRepository = tagGroupRepository ?? throw new ArgumentNullException(nameof(tagGroupRepository));
    }

    #region Repository 属性

    public ITagRepository Tags => _tagRepository;

    public ITagGroupRepository TagGroups => _tagGroupRepository;

    #endregion

    #region 事务管理

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("Transaction is already started.");
        }

        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction to commit.");
        }

        try
        {
            await _context.SaveChangesAsync();
            await _transaction.CommitAsync();
        }
        catch
        {
            await _transaction.RollbackAsync();
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction to rollback.");
        }

        try
        {
            await _transaction.RollbackAsync();
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    #endregion

    #region IDisposable 实现

    private bool _disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context?.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}