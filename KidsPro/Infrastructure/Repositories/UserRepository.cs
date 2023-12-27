﻿using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context, ILogger<BaseRepository<User>> logger) : base(context, logger)
    {
    }

    public async Task<List<User>> GetAllUsersByRole(int role)
    {
        return await _context.Users.ToListAsync();
                                 //   .Include(x=> x.Role).ToListAsync();.Where(x => x.RoleId == role)
    }

    public async Task<User?> GetUserByAttribute(string at1, string? at2, int type)
    {
        switch (type)
        {
            case 1: //Login
                return await _context.Users.Where(x=> x.PhoneNumber.Equals(at1) && x.PasswordHash.Equals(at2))
                                     .FirstOrDefaultAsync();
            case 2: //Search By Name
                break;
        }
        return null;
    }
}