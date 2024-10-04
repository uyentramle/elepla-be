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
	public class QuestionBankConfiguration : IEntityTypeConfiguration<QuestionBank>
	{
		public void Configure(EntityTypeBuilder<QuestionBank> builder)
		{
			builder.ToTable("QuestionBank");
			builder.HasKey(x => x.QuestionId);
		}
	}
}
