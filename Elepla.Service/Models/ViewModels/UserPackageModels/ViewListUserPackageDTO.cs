using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.UserPackageModels
{
    public class ViewListUserPackageDTO
    {
        public string UserPackageId { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string PackageId { get; set; }
        public string PackageName { get; set; }
        public bool UseTemplate { get; set; }
        public bool UseAI { get; set; }
        public bool ExportWord { get; set; }
        public bool ExportPdf { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime DeletedAt { get; set; }
        public string DeletedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
