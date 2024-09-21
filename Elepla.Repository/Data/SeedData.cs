using BCrypt.Net;
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
            await InitializeAccount(unitOfWork);
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

        private static async Task InitializeAccount(IUnitOfWork unitOfWork)
        {
            string password = "123@123";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var adminRole = await unitOfWork.RoleRepository.GetRoleByNameAsync(RoleEnums.Admin.ToString());

            if (adminRole != null)
            {
                var admin = new User
                {
                    UserId = Guid.NewGuid().ToString(),
                    Username = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true,
                    PasswordHash = hashedPassword,
                    Gender = GenderEnums.Unknown.ToString(),
                    Status = true,
                    RoleId = adminRole.RoleId,
                    CreatedAt = DateTime.UtcNow.ToLocalTime(),
                    CreatedBy = "system",
                };

                await unitOfWork.AccountRepository.AddAsync(admin);
            }

            var managerRole = await unitOfWork.RoleRepository.GetRoleByNameAsync(RoleEnums.Manager.ToString());

            if (managerRole != null)
            {
                var manager = new User
                {
                    UserId = Guid.NewGuid().ToString(),
                    Username = "manager",
                    Email = "manager@example.com",
                    EmailConfirmed = true,
                    PasswordHash = hashedPassword,
                    Gender = GenderEnums.Unknown.ToString(),
                    Status = true,
                    RoleId = managerRole.RoleId,
                    CreatedAt = DateTime.UtcNow.ToLocalTime(),
                    CreatedBy = "system",
                };

                await unitOfWork.AccountRepository.AddAsync(manager);
            }
            

            var academicStaffRole = await unitOfWork.RoleRepository.GetRoleByNameAsync(RoleEnums.AcademicStaff.ToString());

            if (academicStaffRole != null)
            {
                var academicStaff = new User
                {
                    UserId = Guid.NewGuid().ToString(),
                    Username = "academicstaff",
                    Email = "academicstaff@example.com",
                    EmailConfirmed = true,
                    PasswordHash = hashedPassword,
                    Gender = GenderEnums.Unknown.ToString(),
                    Status = true,
                    RoleId = academicStaffRole.RoleId,
                    CreatedAt = DateTime.UtcNow.ToLocalTime(),
                    CreatedBy = "system",
                };

                await unitOfWork.AccountRepository.AddAsync(academicStaff);
            }

            var teacherRole = await unitOfWork.RoleRepository.GetRoleByNameAsync(RoleEnums.Teacher.ToString());
            
            if (teacherRole != null)
            {
                var teacher = new User
                {
                    UserId = Guid.NewGuid().ToString(),
                    Username = "teacher",
                    Email = "teacher@example.com",
                    EmailConfirmed = true,
                    PasswordHash = hashedPassword,
                    Gender = GenderEnums.Unknown.ToString(),
                    Status = true,
                    RoleId = teacherRole.RoleId,
                    CreatedAt = DateTime.UtcNow.ToLocalTime(),
                    CreatedBy = "system",
                };

                await unitOfWork.AccountRepository.AddAsync(teacher);
            }

            await unitOfWork.SaveChangeAsync();
        }
    }

}
