using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.FeedbackViewModels
{
    public class ViewFeedbackDTO
    {
        public string FeedbackId { get; set; }
        public string? Content { get; set; }
        public int? Rate { get; set; }
        public string Type { get; set; }
		public bool IsFlagged { get; set; }
        public int FlagCount { get; set; }
        public string TeacherId { get; set; }
        public string TeacherName { get; set; }
        public string Avatar { get; set; }
        public string PlanbookId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set;}
    }
}
