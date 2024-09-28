using Elepla.Service.Models.ResponseModels;
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
    }
}
