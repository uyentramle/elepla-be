using Elepla.Service.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Utils
{
    public class TokenService : ITokenService
    {
        private readonly IMemoryCache _cache;

        public TokenService(IMemoryCache cache)
        {
            _cache = cache;
        }

        // Tạo token và lưu vào cache sau khi xác thực sdt hoặc email thành công của quá trình đăng ký hoặc quên mật khẩu
        public string GenerateToken(string identifier, string method, string type, out DateTime expiryTime)
        {
            var token = Guid.NewGuid().ToString();
            expiryTime = DateTime.UtcNow.ToLocalTime().AddMinutes(10);

            // Lưu token vào cache
            _cache.Set($"{identifier}_{method}_{type}_token", token, TimeSpan.FromMinutes(10));

            return token;
        }

        public bool ValidateToken(string identifier, string method, string type, string token)
        {
            if (_cache.TryGetValue($"{identifier}_{method}_{type}_token", out string? cachedToken) && cachedToken == token)
            {
                // Xóa token khỏi cache ngay sau khi xác thực thành công
                //_cache.Remove($"{identifier}_{method}_{type}_token");
                return true;
            }
            return false;
        }
    }
}
