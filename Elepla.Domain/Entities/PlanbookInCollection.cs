using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Domain.Entities
{
    public class PlanbookInCollection
    {
        // Primary Key
        public string PlanbookInCollectionId { get; set; }

        // Foreign Key
        public string PlanbookId { get; set; }
        public string CollectionId { get; set; }

        // Navigation properties
        public virtual Planbook Planbook { get; set; }
        public virtual PlanbookCollection PlanbookCollection { get; set; }
    }
}
