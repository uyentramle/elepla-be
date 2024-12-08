using Elepla.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.FluentAPIs
{
    public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            builder.ToTable("Activity");
            builder.HasKey(x => x.ActivityId);
            builder.Property(x => x.Objective)
                   .HasMaxLength(int.MaxValue)  // Không giới hạn chiều dài
                   .IsRequired(false); // Trường này có thể null
            builder.Property(x => x.Content)
                   .HasMaxLength(int.MaxValue)  // Không giới hạn chiều dài
                   .IsRequired(false);
            builder.Property(x => x.Product)
                   .HasMaxLength(int.MaxValue)  // Không giới hạn chiều dài
                   .IsRequired(false);
            builder.Property(x => x.Implementation)
                   .HasMaxLength(int.MaxValue)  // Không giới hạn chiều dài
                   .IsRequired(false);
            builder.HasOne(x => x.Planbook)
                .WithMany(x => x.Activities)
                .HasForeignKey(x => x.PlanbookId);
        }
    }
}
