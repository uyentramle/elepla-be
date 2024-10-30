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
	public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
	{
		public void Configure(EntityTypeBuilder<Payment> builder)
		{
			builder.ToTable("Payment");
			builder.HasKey(x => x.PaymentId);
			builder.HasOne(x => x.Package)
				.WithMany(x => x.Payments)
				.HasForeignKey(x => x.PackageId);
			builder.HasOne(x => x.Teacher)
				.WithMany(x => x.Payments)
				.HasForeignKey(x => x.TeacherId);
            builder.Property(x => x.TotalAmount)
               .HasColumnType("decimal(18, 2)");
        }
	}
}
