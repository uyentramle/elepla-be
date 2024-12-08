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
    public class PlanbookInCollectionConfiguration : IEntityTypeConfiguration<PlanbookInCollection>
    {
        public void Configure(EntityTypeBuilder<PlanbookInCollection> builder)
        {
            builder.ToTable("PlanbookInCollection");
            builder.HasKey(x => x.PlanbookInCollectionId);
            builder.HasOne(x => x.Planbook)
                .WithMany(x => x.PlanbookInCollections)
                .HasForeignKey(x => x.PlanbookId);
            builder.HasOne(x => x.PlanbookCollection)
                .WithMany(x => x.PlanbookInCollections)
                .HasForeignKey(x => x.CollectionId);
        }
    }
}
