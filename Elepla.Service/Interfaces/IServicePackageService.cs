using Elepla.Domain.Entities;
using Elepla.Service.Models.ResponseModels;
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
        Task<ResponseModel> GetAllServicePackagesAsync(int pageIndex, int pageSize, string packageName = null);

        // Get a specific service package by its ID
        Task<ResponseModel> GetServicePackageByIdAsync(string packageId);

        // Add a new service package
        Task<ResponseModel> AddServicePackageAsync(CreateServicePackageDTO packageDTO);

        // Update an existing service package
        Task<ResponseModel> UpdateServicePackageAsync(string packageId, UpdateServicePackageDTO packageDTO);

        // Delete a service package by its ID
        Task<ResponseModel> DeleteServicePackageAsync(string packageId);
    }
}
