using Elepla.Domain.Entities;
using Elepla.Repository.Data;
using Elepla.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Repositories
{
    public class ServicePackageRepository : GenericRepository<ServicePackage>, IServicePackageRepository
    {
        public ServicePackageRepository(AppDbContext dbContext, ITimeService timeService, IClaimsService claimsService)
            : base(dbContext, timeService, claimsService)
        {
        }

        // Get all service packages
        public async Task<IEnumerable<ServicePackage>> GetAllServicePackagesAsync()
        {
            return await _dbContext.ServicePackages.ToListAsync();
        }

        // Get a service package by its ID
        public async Task<ServicePackage?> GetServicePackageByIdAsync(string packageId)
        {
            return await _dbContext.ServicePackages.FindAsync(packageId);
        }

        // Add a new service package
        public async Task AddServicePackageAsync(ServicePackage package)
        {
            await _dbContext.ServicePackages.AddAsync(package);
            await _dbContext.SaveChangesAsync();
        }

        // Update an existing service package
        public void UpdateServicePackage(ServicePackage package)
        {
            _dbContext.ServicePackages.Update(package);
            _dbContext.SaveChanges();
        }

        // Delete a service package by its ID
        public void DeleteServicePackage(string packageId)
        {
            var package = _dbContext.ServicePackages.Find(packageId);
            if (package != null)
            {
                _dbContext.ServicePackages.Remove(package);
                _dbContext.SaveChanges();
            }
        }
    }
}
