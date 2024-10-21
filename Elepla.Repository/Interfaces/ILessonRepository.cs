using Elepla.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Interfaces
{
    public interface ILessonRepository : IGenericRepository<Lesson>
    {
        Task<Lesson?> GetLessonByNameAndChapterAsync(string lessonName, string chapterId);
        Task<Lesson?> GetByIdAsync(string lessonId);
    }
}
