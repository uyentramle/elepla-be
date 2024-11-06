using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.ExamViewModels
{
    public class UpdateExamDTO
    {
        [Required]
        public string ExamId { get; set; }

        public string Title { get; set; }
        public string Time { get; set; }
        public List<string> QuestionIds { get; set; } = new List<string>();
    }
}
