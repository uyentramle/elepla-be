using Elepla.Domain.Entities;
using Elepla.Repository.Data;
using Elepla.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Repositories
{
    public class AccountRepository : GenericRepository<User> , IAccountRepository
    {
        //private readonly AppDbContext _dbContext;

        public AccountRepository(AppDbContext dbContext, ITimeService timeService, IClaimsService claimsService) : base(dbContext, timeService, claimsService)
        {
            //_dbContext = dbContext;
        }

        // Lấy người dùng theo email
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        // Lấy người dùng theo tên đăng nhập
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        // Lấy người dùng theo số điện thoại
        public async Task<User?> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        // Lấy người dùng theo email, tên đăng nhập hoặc số điện thoại
        public async Task<User?> GetUserByEmailOrUsernameOrPhoneNumberAsync(string emailOrUsernameOrPhoneNumber)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == emailOrUsernameOrPhoneNumber || u.Username == emailOrUsernameOrPhoneNumber || u.PhoneNumber == emailOrUsernameOrPhoneNumber);
        }
    }
}
