using Elepla.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Interfaces
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role> GetRoleByNameAsync(string roleName);
        Task<IEnumerable<User>> GetUsersInRoleAsync(string roleName);
        Task<bool> RoleExistsAsync(string roleName);
    }
}
