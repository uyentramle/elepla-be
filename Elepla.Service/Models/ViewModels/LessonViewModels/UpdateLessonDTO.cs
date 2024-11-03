﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.LessonViewModels
{
    public class UpdateLessonDTO
    {
        public string LessonId { get; set; }
        public string Name { get; set; }
        public string? Objectives { get; set; }
        public string? Content { get; set; }
        public string ChapterId { get; set; }
    }
}
