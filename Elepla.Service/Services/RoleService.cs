using Elepla.Domain.Entities;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.RoleViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentTime _timeService;

        public RoleService(IUnitOfWork unitOfWork, ICurrentTime timeService)
        {
            _unitOfWork = unitOfWork;
            _timeService = timeService;
        }

        #region Role Management

        // Get all active roles
        public async Task<ResponseModel> GetActiveRolesAsync(int pageIndex, int pageSize)
        {
            var roles = await _unitOfWork.RoleRepository.GetAsync(
                filter: r => !r.IsDeleted,
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Roles retrieved successfully.",
                Data = roles
            };
        }

        // Create a new role
        //public async Task<ResponseModel> CreateRoleAsync(CreateRoleDTO model)
        //{
        //    var existingRole = await _unitOfWork.RoleRepository.GetAsync(r => r.Name == model.RoleName);

        //    if (existingRole != null && !existingRole.IsDeleted)
        //    {
        //        return new ResponseModel
        //        {
        //            Success = false,
        //            Message = "Role already exists.",
        //        };
        //    }

        //    var createBy = "system"; // Lấy thông tin người tạo, có thể từ claims hoặc user hiện tại
        //    var newRole = new Role
        //    {
        //        Name = model.RoleName,
        //        Description = model.Description,
        //        IsDefault = false,
        //        CreatedAt = _timeService.GetCurrentTime(),
        //        CreatedBy = createBy,
        //        IsDeleted = false
        //    };

        //    _unitOfWork.RoleRepository.AddAsync(newRole);
        //    await _unitOfWork.SaveChangeAsync();

        //    return new SuccessResponseModel<string>
        //    {
        //        Success = true,
        //        Message = "Role created successfully.",
        //        Data = newRole.Id.ToString()
        //    };
        //}

        // Update a role
        public async Task<ResponseModel> UpdateRoleAsync(UpdateRoleDTO model)
        {
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(model.Id);

            if (role == null || role.IsDeleted)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "Role not found or has been deleted.",
                };
            }

            role.Name = model.RoleName;
            role.Description = model.Description;
            role.UpdatedAt = _timeService.GetCurrentTime();
            role.UpdatedBy = "system"; // Lấy thông tin người cập nhật

            _unitOfWork.RoleRepository.Update(role);
            await _unitOfWork.SaveChangeAsync();

            return new SuccessResponseModel<string>
            {
                Success = true,
                Message = "Role updated successfully.",
                Data = role.Id.ToString()
            };
        }

        // Delete a role (soft delete)
        public async Task<ResponseModel> DeleteRoleAsync(int roleId)
        {
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(roleId);

            if (role == null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "Role not found.",
                };
            }

            role.IsDeleted = true;
            role.DeletedAt = _timeService.GetCurrentTime();
            role.DeletedBy = "system"; // Lấy thông tin người xóa

            _unitOfWork.RoleRepository.Update(role);
            await _unitOfWork.SaveChangeAsync();

            return new SuccessResponseModel<string>
            {
                Success = true,
                Message = "Role deleted successfully.",
                Data = role.Id.ToString()
            };
        }

        #endregion
    }
}
