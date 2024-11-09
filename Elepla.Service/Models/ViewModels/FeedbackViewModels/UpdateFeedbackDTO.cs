using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.FeedbackViewModels
{
    public class UpdateFeedbackDTO
    {
        [Required]
        public string FeedbackId { get; set; }

        [MaxLength(1000)]
        public string? Content { get; set; }

        [Range(1, 5, ErrorMessage = "Rate must be between 1 and 5.")]
        public int? Rate { get; set; }

        [Required]
        public string TeacherId { get; set; }

        [Required]
        public string PlanbookId { get; set; }

        [Required]
        public string Type { get; set; }
    }

}
