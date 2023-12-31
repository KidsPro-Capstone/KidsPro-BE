﻿using Application;
using Application.Interfaces.IRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;
    private readonly ILogger<UnitOfWork> _logger;

    public IRoleRepository RoleRepository { get; }
    public IUserRepository UserRepository { get; }

    public IRefeshTokenRepository RefeshTokenRepository { get; }
    public ICourseRepository CourseRepository { get; }
    public IClassRepository ClassRepository { get; }

    public UnitOfWork(AppDbContext context, ILogger<UnitOfWork> logger, IRoleRepository roleRepository, IUserRepository userRepository, IRefeshTokenRepository refeshTokenRepository, ICourseRepository courseRepository, IClassRepository classRepository)
    {
        _context = context;
        _logger = logger;
        RoleRepository = roleRepository;
        UserRepository = userRepository;
        RefeshTokenRepository = refeshTokenRepository;
        CourseRepository = courseRepository;
        ClassRepository = classRepository;
    }

    public async Task<int> SaveChangeAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        try
        {
            if (_transaction == null)
                throw new Exception("Transaction is not initiate");
            await _transaction.CommitAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when commit transaction.\nDate:{}", DateTime.UtcNow);
            throw new Exception("Transaction has not been created yet.");
        }
    }

    public async Task RollbackAsync()
    {
        try
        {
            if (_transaction == null)
                throw new Exception("Transaction is not initiate");
            await _transaction.RollbackAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when commit transaction.\nDate:{}", DateTime.UtcNow);
            throw new Exception("Transaction has not been created yet.");
        }
    }

    private bool _disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }

        this._disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}