using Elepla.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Interfaces
{
    public interface IAccountRepository : IGenericRepository<User>
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByPhoneNumberAsync(string phoneNumber);
        Task<User?> GetUserByEmailOrUsernameOrPhoneNumberAsync(string emailOrUsernameOrPhoneNumber);
    }
}
