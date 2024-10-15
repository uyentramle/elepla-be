using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.PlanbookViewModels
{
	public class UpdatePlanbookDTO
	{
		public string PlanbookId { get; set; }
		public string Title { get; set; }
		public string SchoolName { get; set; }
		public string TeacherName { get; set; }
		public string Subject { get; set; }
		public string ClassName { get; set; }
		public int DurationInPeriods { get; set; }
		public string KnowledgeObjective { get; set; }
		public string SkillsObjective { get; set; }
		public string QualitiesObjective { get; set; }
		public string TeachingTools { get; set; }
		public string? Notes { get; set; }

		public string CollectionId { get; set; }
		//public string LessonId { get; set; } 
	}
}
