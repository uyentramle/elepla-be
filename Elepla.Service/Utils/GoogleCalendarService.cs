using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Elepla.Domain.Entities;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Elepla.Service.Interfaces;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Http;

namespace Elepla.Service.Utils
{
    public class GoogleCalendarService : IGoogleCalendarService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private CalendarService _calendarService;

        public GoogleCalendarService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Event> AddEventToCalendarAsync(Event calendarEvent)
        {
            var calendarService = GetCalendarService(); // Method to initialize and return CalendarService
            var insertRequest = calendarService.Events.Insert(calendarEvent, "primary");
            return await insertRequest.ExecuteAsync();
        }

        private CalendarService GetCalendarService()
        {
            // Initialize Google Calendar Service with credentials
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromFile("Resources/credential.json").Secrets,
                new[] { CalendarService.Scope.Calendar },
                "user",
                CancellationToken.None
            //new FileDataStore("Credentials", true),
            //new CustomCodeReceiver("https://eleplaclone-production.up.railway.app/")
            ).Result;

            return new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Elepla"
            });
        }

        // CustomCodeReceiver implementation inside the same class
        private class CustomCodeReceiver : ICodeReceiver
        {
            private readonly string _redirectUri;

            public CustomCodeReceiver(string redirectUri)
            {
                _redirectUri = redirectUri;
            }

            public string RedirectUri => _redirectUri;

            public async Task<AuthorizationCodeResponseUrl> ReceiveCodeAsync(AuthorizationCodeRequestUrl url, CancellationToken cancellationToken)
            {
                // Open the authorization URL in the default browser
                Process.Start(new ProcessStartInfo(url.Build().ToString()) { UseShellExecute = true });

                // Simulate receiving the code manually
                Console.WriteLine("Enter the authorization code:");
                string code = Console.ReadLine();

                // Create and return an AuthorizationCodeResponseUrl instance
                var responseUrl = new AuthorizationCodeResponseUrl
                {
                    Code = code
                };

                return await Task.FromResult(responseUrl);
            }
        }
    }
}
