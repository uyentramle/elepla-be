using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.PlanbookViewModels
{
    public class ViewListPlanbookTemplateDTO
    {
        public string PlanbookId { get; set; }
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Grade { get; set; }
        public string Curriculum { get; set; }
        public string Chapter { get; set; }
        public string Lesson { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
