using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.PaymentViewModels
{
    public class ViewDetailPaymentDTO
    {
        public string PaymentId { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string AddressText { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PackageName { get; set; }
        public string PackageDescription { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
    }

}
