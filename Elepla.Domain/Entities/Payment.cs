using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Domain.Entities
{
	public class Payment : BaseEntity
	{
        // Primary Key
        public string PaymentId { get; set; }

        // Attributes
        public decimal TotalAmount { get; set; }
		public string Status { get; set; }
		public string TeacherId { get; set; }
		//public string UserPackageId { get; set; }
		public string PackageId { get; set; }

		// Navigation properties
		public virtual User Teacher { get; set; }
		//public virtual UserPackage UserPackage { get; set; }
		public virtual ServicePackage Package { get; set; }
	}
}
