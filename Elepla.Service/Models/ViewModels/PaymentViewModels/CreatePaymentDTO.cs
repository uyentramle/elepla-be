using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.PaymentViewModels
{
    public class CreatePaymentDTO
    {
        public string UserId { get; set; }
        public string PackageId { get; set; }
    }

    public class PaymentResultDTO
    {
        public string PaymentId { get; set; }
        public decimal Amount { get; set; }
    }

    public class PaymentLinkDTO
    {
        public string PaymentId { get; set; }
        public string PaymentUrl { get; set; }
        public string QRCode { get; set; }
    }
}
