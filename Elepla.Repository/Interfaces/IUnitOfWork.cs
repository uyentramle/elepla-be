using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepository AccountRepository { get; }
        IRoleRepository RoleRepository { get; }
        Task<int> SaveChangeAsync();
    }
}
