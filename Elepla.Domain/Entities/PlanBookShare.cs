using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Domain.Entities
{
	public class PlanBookShare : BaseEntity
	{
		public string ShareId { get; set; }
		public string ShareType { get; set; }
		public string ShareBy { get; set; } // User Id
		public string ShareTo { get; set; }
		public string AccessLevel { get; set; }
		public string PlanBookId { get; set; }

		public virtual Planbook PlanBook { get; set; }
		public virtual User User { get; set; }

	}
}
