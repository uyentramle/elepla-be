using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.ExamViewModels
{
    public class QuestionDetailDTO
    {
        public string QuestionId { get; set; }
        public string Question { get; set; }
        public string Type { get; set; }
        public string Index { get; set; }
        public List<AnswerDTO> Answers { get; set; } = new List<AnswerDTO>();
    }
}
