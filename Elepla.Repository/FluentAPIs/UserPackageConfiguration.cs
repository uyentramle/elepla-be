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
	public class UserPackageConfiguration : IEntityTypeConfiguration<UserPackage>
	{
		public void Configure(EntityTypeBuilder<UserPackage> builder)
		{
			builder.ToTable("UserPackage");
			builder.HasKey(x => x.id);
			builder.HasOne(x => x.User).WithMany(x => x.UserPackages).HasForeignKey(x => x.UserId);
			builder.HasOne(x => x.Package).WithMany().HasForeignKey(x => x.PackageId);
		}
	}
}
