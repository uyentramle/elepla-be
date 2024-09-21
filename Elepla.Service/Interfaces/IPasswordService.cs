﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Interfaces
{
    public interface IPasswordService
    {
        IEnumerable<string> ValidatePassword(string password);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
