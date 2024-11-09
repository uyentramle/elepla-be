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
    public class CurriculumFrameworkRepository : GenericRepository<CurriculumFramework>, ICurriculumFrameworkRepository
    {
        public CurriculumFrameworkRepository(AppDbContext dbContext, ITimeService timeService, IClaimsService claimsService) : base(dbContext, timeService, claimsService)
        {
        }

        public async Task<CurriculumFramework?> CurriculumFrameworkExistsAsync(string curriculumFrameworkName)
        {
            return await _dbContext.CurriculumFrameworks.FirstOrDefaultAsync(c => c.Name.Equals(curriculumFrameworkName));
        }
    }
}
