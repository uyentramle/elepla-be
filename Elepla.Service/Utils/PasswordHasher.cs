using Elepla.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Utils
{
    public class PasswordHasher : IPasswordHasher
    {
        #region BCrypt
        // Băm mật khẩu
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Xác minh mật khẩu băm với mật khẩu được cung cấp
        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        }
        #endregion

        #region PBKDF2
        // Băm mật khẩu bằng PBKDF2
        public string HashPasswordPBKDF2(string password)
        {
            using (var rfc2898 = new Rfc2898DeriveBytes(password, 16, 10000, HashAlgorithmName.SHA256))
            {
                var salt = rfc2898.Salt;
                var hash = rfc2898.GetBytes(32);
                var hashBytes = new byte[48];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 32);
                return Convert.ToBase64String(hashBytes);
            }
        }

        // Xác minh mật khẩu
        public bool VerifyPasswordPBKDF2(string hashedPassword, string providedPassword)
        {
            var hashBytes = Convert.FromBase64String(hashedPassword);
            var salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            using (var rfc2898 = new Rfc2898DeriveBytes(providedPassword, salt, 10000, HashAlgorithmName.SHA256))
            {
                var hash = rfc2898.GetBytes(32);
                for (int i = 0; i < 32; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        #endregion
    }
}
