namespace Elepla.Service.Models.ViewModels.SubjectInCurriculumViewModels
{
    public class ViewListSubjectInCurriculumDTO
    {
        public string SubjectInCurriculumId { get; set; }
        public string Description { get; set; }
        public string SubjectId { get; set; }
        public string CurriculumId { get; set; }
        public string GradeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime DeletedAt { get; set; }
        public string DeletedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
