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
	public class ArticleConfiguration : IEntityTypeConfiguration<Article>
	{
		public void Configure(EntityTypeBuilder<Article> builder)
		{
			builder.ToTable("Article");
			builder.HasKey(x => x.ArticleId);
		}
	}
}
