using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.AccountViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Interfaces
{
    public interface IAccountService
    {
        Task<ResponseModel> GetUserProfileAsync(string userId);
        Task<ResponseModel> UpdateUserProfileAsync(UpdateUserProfileDTO model);
    }
}
