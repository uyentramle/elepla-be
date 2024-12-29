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
	public class PlanbookShareConfiguration : IEntityTypeConfiguration<PlanbookShare>
	{
		public void Configure(EntityTypeBuilder<PlanbookShare> builder)
		{
			builder.ToTable("PlanbookShare");
			builder.HasKey(x => x.ShareId);
			builder.HasOne(x => x.Planbook)
				.WithMany(x => x.PlanbookShares)
				.HasForeignKey(x => x.PlanbookId)
				.OnDelete(DeleteBehavior.Cascade); // Xóa PlanbookShares khi xóa Planbook
			builder.HasOne(x => x.SharedByUser)
				.WithMany(x => x.SharedPlanbooks)
				.HasForeignKey(x => x.SharedBy)
				.OnDelete(DeleteBehavior.Restrict);
			builder.HasOne(x => x.SharedToUser)
				.WithMany(x => x.ReceivedPlanbooks)
				.HasForeignKey(x => x.SharedTo)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
