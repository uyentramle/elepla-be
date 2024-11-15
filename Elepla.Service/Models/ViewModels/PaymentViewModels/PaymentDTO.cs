using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.PaymentViewModels
{
    public class PaymentDTO
    {
        public string PaymentId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string TeacherId { get; set; }
        public string Fullname { get; set; }
        public string PackageName { get; set; } 
        public DateTime CreatedAt { get; set; } 
    }

}
