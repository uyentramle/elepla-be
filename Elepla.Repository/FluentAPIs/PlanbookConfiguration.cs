﻿using Elepla.Domain.Entities;
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
            builder.Property(x => x.SchoolName).HasMaxLength(50);
            builder.Property(x => x.TeacherName).HasMaxLength(100);
            builder.Property(x => x.ClassName).HasMaxLength(20);
            builder.Property(x => x.KnowledgeObjective)
                   .HasMaxLength(int.MaxValue)  // Không giới hạn chiều dài
                   .IsRequired(false); // Trường này có thể null
            builder.Property(x => x.SkillsObjective)
                   .HasMaxLength(int.MaxValue)  
                   .IsRequired(false); 
            builder.Property(x => x.QualitiesObjective)
                   .HasMaxLength(int.MaxValue)  
                   .IsRequired(false); 
            builder.Property(x => x.TeachingTools)
                   .HasMaxLength(int.MaxValue)  
                   .IsRequired(false);
            builder.Property(x => x.Notes)
                   .HasMaxLength(int.MaxValue)
                   .IsRequired(false); 
            //builder.HasOne(x => x.PlanbookCollection)
            //    .WithMany(x => x.Planbooks)
            //    .HasForeignKey(x => x.CollectionId)
            //    .OnDelete(DeleteBehavior.Restrict); // Sẽ không xóa PlanbookCollection khi xóa Planbook
            builder.HasOne(x => x.Lesson)
                .WithMany(x => x.Planbooks)
                .HasForeignKey(x => x.LessonId)
                .OnDelete(DeleteBehavior.Restrict); // Sẽ không xóa Lesson khi xóa Planbook
            builder.HasMany(x => x.Activities)
                .WithOne(x => x.Planbook)
                .HasForeignKey(x => x.PlanbookId)
                .OnDelete(DeleteBehavior.Cascade); // Sẽ tự động xóa các Activity khi xóa Planbook
            builder.HasMany(x => x.TeachingSchedules)
                .WithOne(x => x.Planbook)
                .HasForeignKey(x => x.PlanbookId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa TeachingSchedules khi xóa Planbook
            builder.HasMany(x => x.Feedbacks)
                .WithOne(x => x.Planbook)
                .HasForeignKey(x => x.PlanbookId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Feedbacks khi xóa Planbook
            builder.HasMany(x => x.PlanbookShares)
                .WithOne(x => x.Planbook)
                .HasForeignKey(x => x.PlanbookId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa PlanbookShares khi xóa Planbook
        }
    }
}
