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
    public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.ToTable("Feedback");
            builder.HasKey(x => x.FeedbackId);
            builder.Property(x => x.Content).HasMaxLength(1000);
            builder.Property(x => x.Rate).IsRequired();
            builder.Property(x => x.Type)
                .HasMaxLength(50)
                .IsRequired();
            builder.HasOne(x => x.Teacher)
                .WithMany(x => x.Feedbacks)
                .HasForeignKey(x => x.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Planbook)
                .WithMany(x => x.Feedbacks)
                .HasForeignKey(x => x.PlanbookId)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa cascade
        }
    }
}
