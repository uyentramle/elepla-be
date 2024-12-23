﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.LessonViewModels
{
    public class ViewListLessonDTO
    {
        public string LessonId { get; set; }
        public string Name { get; set; }
        public string? Objectives { get; set; }
        public string? Content { get; set; }
        public string ChapterId { get; set; }
        public string ChapterName { get; set; }
        public string SubjectId { get; set; }
        public string Subject { get; set; }
        public string GradeId { get; set; }
        public string Grade { get; set; }
        public string CurriculumId { get; set; }
        public string Curriculum { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime DeletedAt { get; set; }
        public string DeletedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
