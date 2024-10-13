using Elepla.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Utils
{
    public class TimeService : ITimeService
    {
        public DateTime GetCurrentTime() => DateTime.UtcNow.ToLocalTime();
    }
}
