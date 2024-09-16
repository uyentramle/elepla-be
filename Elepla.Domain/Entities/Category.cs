using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Domain.Entities
{
	public class Category : BaseEntity
	{
		public string CategoryId { get; set; }
		public string Name { get; set; }
		public string URL { get; set; }
		public string Description { get; set; }
		public bool Status { get; set; }
		
	}
}
