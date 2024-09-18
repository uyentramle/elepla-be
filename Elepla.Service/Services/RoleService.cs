using AutoMapper;
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
        private readonly IMapper _mapper;

        public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
        public async Task<ResponseModel> CreateRoleAsync(CreateRoleDTO model)
        {
            var existingRole = await _unitOfWork.RoleRepository.GetRoleByNameAsync(model.RoleName);

            if (existingRole != null)
            {
                if (existingRole.IsDeleted)
                {
                    // Role đã bị xóa mềm, cập nhật lại role này
                    existingRole.Description = model.Description;
                    existingRole.IsDeleted = false;
                    _unitOfWork.RoleRepository.Update(existingRole);
                    await _unitOfWork.SaveChangeAsync();

                    return new ResponseModel
                    {
                        Success = true,
                        Message = "Role restored successfully.",
                    };
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = "Role already exists.",
                };
            }

            var newRole = new Role
            {
                Name = model.RoleName,
                Description = model.Description,
                IsDefault = false,
                IsDeleted = false
            };

            await _unitOfWork.RoleRepository.AddAsync(newRole);
            await _unitOfWork.SaveChangeAsync();

            return new SuccessResponseModel<string>
            {
                Success = true,
                Message = "Role created successfully.",
                Data = newRole.Name
            };
        }

        // Update a role
        public async Task<ResponseModel> UpdateRoleAsync(UpdateRoleDTO model)
        {
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(model.Id);

            if (role is null || role.IsDeleted)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "Role not found or has been deleted.",
                };
            }

            role.Name = model.RoleName;
            role.Description = model.Description;

            _unitOfWork.RoleRepository.Update(role);
            await _unitOfWork.SaveChangeAsync();

            return new SuccessResponseModel<string>
            {
                Success = true,
                Message = "Role updated successfully.",
                Data = role.Name
            };
        }

        // Delete a role (soft delete)
        public async Task<ResponseModel> DeleteRoleAsync(int roleId)
        {
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(roleId);

            if (role is null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "Role not found.",
                };
            }

            // Kiểm tra xem role có phải là role mặc định không
            if (role.IsDefault)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "Cannot delete default role.",
                };
            }

            _unitOfWork.RoleRepository.SoftRemove(role);
            await _unitOfWork.SaveChangeAsync();

            return new ResponseModel
            {
                Success = true,
                Message = "Role deleted successfully.",
            };
        }
        #endregion
    }
}
