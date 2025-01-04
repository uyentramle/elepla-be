using Elepla.Service.Models.ViewModels.AnswerViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.QuestionBankViewModels
{
	public class CreateQuestionDTO
	{
		public string Question { get; set; }
		public string Type { get; set; }
		public string Plum { get; set; }
        public bool IsDefault { get; set; }
        public string ChapterId { get; set; }
		public string? LessonId { get; set; }
		public List<CreateAnswerDTO>? Answers { get; set; }
	}
}
