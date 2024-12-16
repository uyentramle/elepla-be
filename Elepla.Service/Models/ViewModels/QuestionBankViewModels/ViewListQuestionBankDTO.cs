﻿using Elepla.Service.Models.ViewModels.AnswerViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.QuestionBankViewModels
{
	public class ViewListQuestionBankDTO
	{
		public string QuestionId { get; set; }
		public string Question { get; set; }
		public string Type { get; set; }
		public string Plum { get; set; }
        public string Subject { get; set; }
        public string Curriculum { get; set; }
        public string Grade { get; set; }
        public string ChapterId { get; set; }
        public string ChapterName { get; set; }
        public string LessonId { get; set; }
        public string LessonName { get; set; }
        public List<ViewListAnswerDTO> Answers { get; set; }

		public DateTime CreatedAt { get; set; }
		public string CreatedBy { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public string UpdatedBy { get; set; }
		public DateTime? DeletedAt { get; set; }
		public string? DeletedBy { get; set; }
		public bool IsDeleted { get; set; }
	}
}
