using Elepla.Domain.Entities;
using Elepla.Repository.FluentAPIs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ServicePackage> ServicePackages { get; set; }
		public DbSet<Payment> Payments { get; set; }
        public DbSet<UserPackage> UserPackages { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new ImageConfiguration());
			modelBuilder.ApplyConfiguration(new ServicePackageConfiguration());
			modelBuilder.ApplyConfiguration(new PaymentConfiguration());
			modelBuilder.ApplyConfiguration(new UserPackageConfiguration());
			modelBuilder.ApplyConfiguration(new ArticleConfiguration());
			modelBuilder.ApplyConfiguration(new CategoryConfiguration());
		}
    }
}
