using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.RoleViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Interfaces
{
    public interface IRoleService
    {
        Task<ResponseModel> GetActiveRolesAsync(int pageIndex, int pageSize);
        Task<ResponseModel> CreateRoleAsync(CreateRoleDTO model);
        Task<ResponseModel> UpdateRoleAsync(UpdateRoleDTO model);
        Task<ResponseModel> DeleteRoleAsync(int roleId);
    }
}
