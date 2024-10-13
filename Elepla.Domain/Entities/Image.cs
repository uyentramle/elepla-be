﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Domain.Entities
{
	public class Image : BaseEntity
	{
		// Primary Key
		public string ImageId { get; set; }

		// Attributes
		public string ImageUrl { get; set; }
		public string Type { get; set; }

		// Navigation properties
		public virtual ICollection<User> UserAvatars { get; set; } = new List<User>();
		public virtual ICollection<User> UserBackgrounds { get; set; } = new List<User>();
        public virtual ICollection<ArticleImage> ArticleImages { get; set; } = new List<ArticleImage>();
    }
}
