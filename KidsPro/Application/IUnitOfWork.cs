﻿using Application.Interfaces.IRepositories;

namespace Application;

public interface IUnitOfWork : IDisposable
{
    public IRoleRepository RoleRepository { get; }
    public IUserRepository UserRepository { get; }
    public IRefeshTokenRepository RefeshTokenRepository { get; }
    
    public ICourseRepository CourseRepository { get; }
    public IClassRepository ClassRepository { get;  }

    public Task<int> SaveChangeAsync();

    public Task BeginTransactionAsync();
    public Task CommitAsync();
    public Task RollbackAsync();
}