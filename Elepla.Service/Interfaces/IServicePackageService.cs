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
        Task<ResponseModel> GetAllServicePackagesAsync(string? keyword, int pageIndex, int pageSize);

        // Get a specific service package by its ID
        Task<ResponseModel> GetServicePackageByIdAsync(string packageId);

        // Add a new service package
        Task<ResponseModel> AddServicePackageAsync(CreateServicePackageDTO model);

        // Update an existing service package
        Task<ResponseModel> UpdateServicePackageAsync(UpdateServicePackageDTO model);

        // Delete a service package by its ID
        Task<ResponseModel> DeleteServicePackageAsync(string packageId);
    }
}
