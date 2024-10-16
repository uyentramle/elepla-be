using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Domain.Entities
{
    public class PlanbookCollection : BaseEntity
    {
        // Primary Key
        public string CollectionId { get; set; }

        // Attributes
        public string CollectionName { get; set; }
        //public string CollectionType { get; set; }

		// Foreign Key
		public string TeacherId { get; set; }

        // Navigation properties
        public virtual User Teacher { get; set; }
        public virtual ICollection<Planbook> Planbooks { get; set; } = new List<Planbook>();
    }
}
