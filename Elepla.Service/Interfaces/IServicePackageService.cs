using Elepla.Domain.Entities;
using Elepla.Service.Models.ViewModels.ServicePackageViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Interfaces
{
    public interface IServicePackageService
    {
        // Get all service packages
        Task<IEnumerable<ServicePackage>> GetAllServicePackagesAsync();

        // Get a specific service package by its ID
        Task<ServicePackage?> GetServicePackageByIdAsync(string packageId);

        // Add a new service package
        Task AddServicePackageAsync(CreateServicePackageDTO packageDTO);

        // Update an existing service package
        Task UpdateServicePackageAsync(string packageId, UpdateServicePackageDTO packageDTO);

        // Delete a service package by its ID
        Task DeleteServicePackageAsync(string packageId);
    }
}
