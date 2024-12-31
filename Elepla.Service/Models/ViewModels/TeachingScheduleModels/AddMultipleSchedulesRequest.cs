using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.TeachingScheduleModels
{
    public class AddMultipleSchedulesRequest
    {
        public List<string> ScheduleIds { get; set; } = new List<string>();
        public string AuthorizationCode { get; set; }
    }

}
