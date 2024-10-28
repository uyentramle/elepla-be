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
    public class ChapterRepository : GenericRepository<Chapter>, IChapterRepository
    {
        public ChapterRepository(AppDbContext dbContext, ITimeService timeService, IClaimsService claimsService) : base(dbContext, timeService, claimsService)
        {
        }

        public async Task<Chapter?> GetChapterByNameAndSubjectInCurriculumAsync(string chapterName, string subjectInCurriculumId)
        {
            return await _dbContext.Chapters.FirstOrDefaultAsync(c => c.Name.Equals(chapterName) && c.SubjectInCurriculumId.Equals(subjectInCurriculumId));
        }
    }
}
