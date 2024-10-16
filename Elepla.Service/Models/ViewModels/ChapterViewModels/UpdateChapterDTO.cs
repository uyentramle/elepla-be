using System.ComponentModel.DataAnnotations;

namespace Elepla.Service.Models.ViewModels.ChapterViewModels

{
    public class UpdateChapterDTO
    {
        public string ChapterId { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string SubjectInCurriculumId { get; set; }

    }
}
