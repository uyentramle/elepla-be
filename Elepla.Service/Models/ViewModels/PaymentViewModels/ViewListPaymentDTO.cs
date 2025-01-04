using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.PaymentViewModels
{
    public class ViewListPaymentDTO
    {
        public string PaymentId { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentUrl { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string PackageId { get; set; }
        public string PackageName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
