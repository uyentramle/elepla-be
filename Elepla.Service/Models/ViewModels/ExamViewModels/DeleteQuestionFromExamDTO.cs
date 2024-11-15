using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.ExamViewModels
{
    public class DeleteQuestionFromExamDTO
    {
        public List<string> QuestionIds { get; set; } = new List<string>();
    }
}
