using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Domain.Entities
{
	public class Article : BaseEntity
	{
		public string ArticleId { get; set; }
		public string URL { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public string Status { get; set; }

	}
}
