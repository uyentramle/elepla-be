using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.ExamViewModels
{
    public class CreateExamDTO
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Time { get; set; }

        [Required]
        public string UserId { get; set; }

        public List<string> QuestionIds { get; set; } = new List<string>();
    }
}
