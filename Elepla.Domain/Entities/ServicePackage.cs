﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Domain.Entities
{
	public class ServicePackage : BaseEntity
	{
        // Primary Key
        public string PackageId { get; set; }

        // Attributes
        public string PackageName { get; set; }
		public string? Description { get; set; }
		public decimal Price { get; set; }
		public decimal Discount { get; set; }
		public int Duration { get; set; }
		public int MaxLessonPlans { get; set; }

        // Navigation properties
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<UserPackage> UserPackages { get; set; } = new List<UserPackage>();
    }
}
