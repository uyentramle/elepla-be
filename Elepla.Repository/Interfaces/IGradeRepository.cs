using Elepla.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Interfaces
{
    public interface IGradeRepository : IGenericRepository<Grade>
    {
        Task<bool> GradeExistsAsync(string gradeName);
    }
}
