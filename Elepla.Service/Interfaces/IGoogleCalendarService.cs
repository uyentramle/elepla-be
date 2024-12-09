using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Interfaces
{
    public interface IGoogleCalendarService
    {
        Task<string> CreateEventAsync(string calendarId, string title, string description, DateTime start, DateTime end, string timeZone);
        Task<IList<CalendarListEntry>> GetCalendarListAsync();
        Task<Calendar> CreateAsync(Calendar newCalendar);
        Task InitializeServiceFromCredentialFileAsync(string userTokenPath);
    }
}
