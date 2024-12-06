﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.PlanbookViewModels
{
	public class ViewListPlanbookDTO
	{
		public string PlanbookId { get; set; }

		public string Title { get; set; }
		public string SchoolName { get; set; }
		public string TeacherName { get; set; }
		public string Subject { get; set; }
		public string ClassName { get; set; }
		public bool IsPublic { get; set; }
        //public int DurationInPeriods { get; set; }
        //public string KnowledgeObjective { get; set; }
        //public string SkillsObjective { get; set; }
        //public string QualitiesObjective { get; set; }
        //public string TeachingTools { get; set; }
        //public string? Notes { get; set; }

		//public string CollectionId { get; set; }
		//public string CollectionName { get; set; }
		public string LessonId { get; set; }
		public string LessonName { get; set; }

		public DateTime CreatedAt { get; set; }
		public string CreatedBy { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public string UpdatedBy { get; set; }
		public DateTime? DeletedAt { get; set; }
		public string? DeletedBy { get; set; }
		public bool IsDeleted { get; set; }
	}
}
