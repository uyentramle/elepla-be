using Elepla.Domain.Entities;
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
        string GenerateAuthorizationUrl(string scheduleId);
        Task<Event> AddEventToCalendarAfterAuthorizationAsync(string authorizationCode, Event calendarEvent);
    }
}
