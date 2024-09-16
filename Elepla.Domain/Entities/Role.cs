using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Domain.Entities
{
    public class Role : BaseEntity
    {
        public Role(string name, string description, bool isDefault, DateTime createdAt, string createdBy, bool isDeleted)
        {
            Name = name;
            Description = description;
            IsDefault = isDefault;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
            IsDeleted = isDeleted;
        }

        // Primary Key
        public int Id { get; set; }

        // Attributes
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsDefault { get; set; }

        // Navigation properties
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
