using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Interfaces
{
    public interface IPayOSService
    {
        Task<CreatePaymentResult> CreatePaymentLink(PaymentData paymentData);
        Task<PaymentLinkInformation> GetPaymentLinkInformation(int orderId);
        Task<PaymentLinkInformation> CancelPaymentLink(int paymentId);
        Task<string> ConfirmWebhook(string webhookUrl);
        WebhookData VerifyPaymentWebhookData(WebhookType webhookBody);
    }
}
