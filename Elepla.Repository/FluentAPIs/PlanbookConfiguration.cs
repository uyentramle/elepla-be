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
    public class PlanbookConfiguration : IEntityTypeConfiguration<Planbook>
    {
        public void Configure(EntityTypeBuilder<Planbook> builder)
        {
            builder.ToTable("Planbook");
            builder.HasKey(x => x.PlanbookId);
            builder.Property(x => x.SchoolName).HasMaxLength(50).IsRequired();
            builder.Property(x => x.TeacherName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.ClassName).HasMaxLength(20).IsRequired();
            builder.HasOne(x => x.PlanbookCollection)
                .WithMany(x => x.Planbooks)
                .HasForeignKey(x => x.CollectionId);
            builder.HasOne(x => x.Lesson)
                .WithMany(x => x.Planbooks)
                .HasForeignKey(x => x.LessonId);
            builder.HasMany(x => x.Activities)
                .WithOne(x => x.Planbook)
                .HasForeignKey(x => x.PlanbookId);
            builder.HasMany(x => x.TeachingSchedules)
                .WithOne(x => x.Planbook)
                .HasForeignKey(x => x.PlanbookId);
            builder.HasMany(x => x.Feedbacks)
                .WithOne(x => x.Planbook)
                .HasForeignKey(x => x.PlanbookId);
            builder.HasMany(x => x.PlanbookShares)
                .WithOne(x => x.PlanBook)
				.HasForeignKey(x => x.PlanBookId);
		}
    }
}
