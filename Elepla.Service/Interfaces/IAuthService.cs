﻿using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.AuthViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseModel> LoginAsync(LoginDTO model);
    }
}