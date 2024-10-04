using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Domain.Entities
{
	public class QuestionBank : BaseEntity
	{
		public string QuestionId { get; set; }
		public string Question { get; set; }
		public string Type { get; set; }
		public string Plum { get; set; }

	}
}
