using Elepla.Service.Models.ViewModels.AnswerViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.ExamViewModels
{
    public class ViewDetailExamDTO
    {
        public string ExamId { get; set; }
        public string Title { get; set; }
        public string Time { get; set; }
        public string UserId { get; set; }
        public List<QuestionInExamDTO> Questions { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class QuestionInExamDTO
    {
        public string QuestionId { get; set; }
        public string Question { get; set; }
        public string Type { get; set; }
        public string Plum { get; set; }
        public int Index { get; set; }
        public List<ViewListAnswerDTO> Answers { get; set; }
    }
}
