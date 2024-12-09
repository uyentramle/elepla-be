using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Elepla.Service.Interfaces;
using Google.Apis.Util.Store;

namespace Elepla.Service.Utils
{
    public class GoogleCalendarService : IGoogleCalendarService
    {
        private CalendarService _calendarService;

        public GoogleCalendarService()
        {
            // Path to the service account key file
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "service-account.json");

            if (!File.Exists(jsonPath))
            {
                throw new FileNotFoundException("Google service account key file not found.", jsonPath);
            }

            // Create credentials from the service account JSON key file
            GoogleCredential credential;
            using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(CalendarService.Scope.Calendar);
            }

            // Initialize the Calendar service
            _calendarService = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Elepla"
            });
        }

        public async Task InitializeServiceFromCredentialFileAsync(string userTokenPath)
        {
            // Path to the credential file
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "credential.json");

            if (!File.Exists(jsonPath))
            {
                throw new FileNotFoundException("Google OAuth credential file not found.", jsonPath);
            }

            // Load the credential.json file
            using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
            {
                //var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                //    GoogleClientSecrets.FromStream(stream).Secrets,
                //    new[] { CalendarService.Scope.Calendar },
                //    "user", // The user to authenticate
                //    System.Threading.CancellationToken.None,
                //    new Google.Apis.Util.Store.FileDataStore(userTokenPath, true)
                //);

                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    new[] { CalendarService.Scope.Calendar },
                    "user", // The user to authenticate
                    System.Threading.CancellationToken.None,
                    new FileDataStore(userTokenPath, true), // Specify token storage
                    new LocalServerCodeReceiver() // Default receiver (you can override if necessary)
                );

                // Initialize the Calendar service
                _calendarService = new CalendarService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Elepla"
                });
            }
        }

        public async Task<string> CreateEventAsync(string calendarId, string title, string description, DateTime start, DateTime end, string timeZone)
        {
            var calendarEvent = new Event
            {
                Summary = title,
                Description = description,
                Start = new EventDateTime
                {
                    DateTime = start,
                    TimeZone = timeZone
                },
                End = new EventDateTime
                {
                    DateTime = end,
                    TimeZone = timeZone
                },
            };

            var request = _calendarService.Events.Insert(calendarEvent, calendarId);
            var createdEvent = await request.ExecuteAsync();

            return createdEvent.Id; // Return the event ID
        }

        public async Task<IList<CalendarListEntry>> GetCalendarListAsync()
        {
            var calendarList = await _calendarService.CalendarList.List().ExecuteAsync();
            return calendarList.Items;
        }

        public async Task<Calendar> CreateAsync(Calendar newCalendar)
        {
            var request = _calendarService.Calendars.Insert(newCalendar);
            var createdCalendar = await request.ExecuteAsync();
            return createdCalendar;
        }
    }
}
