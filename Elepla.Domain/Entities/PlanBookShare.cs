using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Domain.Entities
{
	public class PlanbookShare : BaseEntity
	{
        // Primary Key
        public string ShareId { get; set; }

        // Attributes
        //public string ShareType { get; set; }
        //public string? ShareTo { get; set; } // User Id
        //public string? ShareToEmail { get; set; }
        public bool IsEdited { get; set; }

        // Foreign Key
        public string PlanbookId { get; set; }
		public string SharedBy { get; set; } // User Id thực hiện chia sẻ
        public string SharedTo { get; set; } // User Id nhận chia sẻ

        // Navigation properties
        public virtual Planbook Planbook { get; set; }
		public virtual User SharedByUser { get; set; }
        public virtual User SharedToUser { get; set; }
    }
}
