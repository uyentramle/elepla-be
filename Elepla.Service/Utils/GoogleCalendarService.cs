using Elepla.Service.Common;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ViewModels.AuthViewModels;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Elepla.Service.Utils
{
    public class GoogleCalendarService : IGoogleCalendarService
    {
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string redirectUri;
        private readonly IMemoryCache _cache;


        public GoogleCalendarService(IMemoryCache cache)
        {
            _cache = cache;

            // Đọc thông tin từ credential.json
            var credentialPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "credential.json");
            var credentialJson = File.ReadAllText(credentialPath);
            var credentialData = JObject.Parse(credentialJson);

            clientId = credentialData["web"]["client_id"]?.ToString();
            clientSecret = credentialData["web"]["client_secret"]?.ToString();
            redirectUri = credentialData["web"]["redirect_uris"]?[0]?.ToString();
        }

        public string GenerateAuthorizationUrl(string scheduleId)
        {
            // Tạo URL xác thực Google OAuth 2.0 với state chứa scheduleId để nhận diện
            var authorizationUrl = new UriBuilder("https://accounts.google.com/o/oauth2/v2/auth")
            {
                Query = $"client_id={clientId}" +
                        $"&redirect_uri={redirectUri}" +
                        $"&response_type=code" +
                        $"&scope=https://www.googleapis.com/auth/calendar" +
                        $"&state={scheduleId}" +
                        $"&access_type=offline" +
                        $"&prompt=consent"
            };

            return authorizationUrl.ToString();
        }

        public async Task<Event> AddEventToCalendarAfterAuthorizationAsync(string authorizationCode, Event calendarEvent)
        {
            // Lấy Access Token từ Authorization Code
            string accessToken = await GetAccessTokenFromAuthorizationCode(authorizationCode);

            // Tạo dịch vụ Google Calendar
            var calendarService = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GoogleCredential.FromAccessToken(accessToken),
                ApplicationName = "Elepla"
            });

            // Thêm sự kiện vào Google Calendar
            var insertRequest = calendarService.Events.Insert(calendarEvent, "primary");
            return await insertRequest.ExecuteAsync();
        }
        
        private async Task<string> GetAccessTokenFromAuthorizationCode(string authorizationCode)
        {
            using (var httpClient = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    { "code", authorizationCode },
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "redirect_uri", redirectUri },
                    { "grant_type", "authorization_code" }
                };

                var content = new FormUrlEncodedContent(values);
                var response = await httpClient.PostAsync("https://oauth2.googleapis.com/token", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Không thể lấy Access Token: {responseContent}");
                }

                var tokenData = JObject.Parse(responseContent);
                return tokenData["access_token"]?.ToString();
            }
        }
    }
}
