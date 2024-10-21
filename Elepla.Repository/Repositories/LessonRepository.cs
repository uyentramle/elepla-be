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

        public async Task<Lesson?> GetByIdAsync(string lessonId)
        {
            return await _dbContext.Lessons
                .Include(l => l.Chapter)                                  // Include Chapter
                .ThenInclude(c => c.SubjectInCurriculum)                  // Include SubjectInCurriculum
                .ThenInclude(s => s.Subject)                              // Include Subject
                .ThenInclude(g => g.SubjectInCurriculums)                 // Include Subject's Grade
                .Include(l => l.Chapter.SubjectInCurriculum.Grade)        // Include Grade
                .FirstOrDefaultAsync(l => l.LessonId == lessonId);
        }
    }
}
