using Elepla.Domain.Entities;
using Elepla.Domain.Enums;
using Elepla.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Data
{
    public class SeedData
    {
        public async Task Initialize(IUnitOfWork unitOfWork)
        {
            await InitializeRole(unitOfWork);
        }

        private static async Task InitializeRole(IUnitOfWork unitOfWork)
        {
            var roles = Enum.GetNames(typeof(RoleEnums)); // Lấy danh sách tên các vai trò từ enum RoleEnums
            foreach (var roleName in roles)
            {
                var roleExists = await unitOfWork.RoleRepository.RoleExistsAsync(roleName); // Kiểm tra xem vai trò đã tồn tại chưa
                if (!roleExists)
                {
                    var role = new Role
                    {
                        Name = roleName,
                        IsDefault = true,
                        CreatedAt = DateTime.UtcNow.ToLocalTime(),
                        CreatedBy = "system",
                        IsDeleted = false
                    };

                    await unitOfWork.RoleRepository.AddAsync(role); 
                }
            }

            await unitOfWork.SaveChangeAsync(); 
        }
    }

}
