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
    public class LessonRepository : GenericRepository<Lesson>, ILessonRepository
    {
        public LessonRepository(AppDbContext dbContext, ITimeService timeService, IClaimsService claimsService) : base(dbContext, timeService, claimsService)
        {
        }

        public async Task<Lesson?> GetLessonByNameAndChapterAsync(string lessonName, string chapterId)
        {
            return await _dbContext.Lessons.FirstOrDefaultAsync(l => l.Name.Equals(lessonName) && l.ChapterId.Equals(chapterId));
        }
    }
}
