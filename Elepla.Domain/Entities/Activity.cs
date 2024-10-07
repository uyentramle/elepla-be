﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Domain.Entities
{
    public class Activity
    {
        // Primary Key
        public string ActivityId { get; set; }

        // Attributes
        public string Title { get; set; }
        public string? Objective { get; set; }
        public string? Content { get; set; }
        public string? Product { get; set; }
        public string? Implementation { get; set; }
        public int Index { get; set; }

        // Foreign Key
        public string PlanbookId { get; set; }

        // Navigation properties
        public virtual Planbook Planbook { get; set; }
    }
}
