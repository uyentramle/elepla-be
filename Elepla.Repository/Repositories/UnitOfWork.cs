using Elepla.Repository.Data;
using Elepla.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private readonly IRoleRepository _roleRepository;

        public UnitOfWork(AppDbContext dbContext,
            IRoleRepository roleRepository)
        {
            _dbContext = dbContext;
            _roleRepository = roleRepository;
        }

        public IRoleRepository RoleRepository => _roleRepository;

        public async Task<int> SaveChangeAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
