using Elepla.Service.Models.ViewModels.ServicePackageViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.UserPackageModels
{
    public class UserPackageDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string PackageId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public ServicePackageDTO Package { get; set; } 
    }
}
