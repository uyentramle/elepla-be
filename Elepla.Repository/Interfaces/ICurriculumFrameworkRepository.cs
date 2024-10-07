using Elepla.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Interfaces
{
    public interface ICurriculumFrameworkRepository : IGenericRepository<CurriculumFramework>
    {
        Task<bool> CurriculumFrameworkExistsAsync(string curriculumFrameworkName);
    }
}
