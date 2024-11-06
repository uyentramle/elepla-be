﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.ExamViewModels
{
    public class ViewExamDTO
    {
        public string ExamId { get; set; }
        public string Title { get; set; }
        public string Time { get; set; }
        public string UserId { get; set; }
        public List<QuestionDetailDTO> Questions { get; set; } = new List<QuestionDetailDTO>();
    }
}
