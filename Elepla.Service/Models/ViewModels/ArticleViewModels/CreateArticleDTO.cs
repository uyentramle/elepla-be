using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.ArticleViewModels
{
	public class CreateArticleDTO
	{
		[Required(ErrorMessage = "Title is required.")]
		public string Title { get; set; }
		public string? Content { get; set; }
		public string Status { get; set; }
	}
}
