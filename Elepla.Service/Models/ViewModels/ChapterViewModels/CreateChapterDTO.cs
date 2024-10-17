using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.ChapterViewModels
{
    public class CreateChapterDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string SubjectInCurriculumId { get; set; }
    }
}
