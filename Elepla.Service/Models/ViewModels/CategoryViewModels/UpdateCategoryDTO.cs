﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.CategoryViewModels
{
	public class UpdateCategoryDTO
	{
		public string Id { get; set; }
		[Required(ErrorMessage = "Name is required.")]
		public string Name { get; set; }
		public string? Url { get; set; }
		public string? Description { get; set; }
		public bool Status { get; set; }
	}
}
