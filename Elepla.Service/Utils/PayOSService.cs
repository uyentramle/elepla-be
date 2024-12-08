using Elepla.Service.Common;
using Elepla.Service.Interfaces;
using Net.payOS;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Utils
{
    public class PayOSService : IPayOSService
    {
        private readonly PayOS _payOS;

        public PayOSService(AppConfiguration appConfiguration)
        {
            var payOSSettings = appConfiguration.PayOS;
            _payOS = new PayOS(payOSSettings.ClientId, payOSSettings.ApiKey, payOSSettings.CheckSumKey);
        }

        // Method to create a payment link
        public async Task<CreatePaymentResult> CreatePaymentLink(PaymentData paymentData)
        {
            return await _payOS.createPaymentLink(paymentData);
        }

        // Method to get payment link information
        public async Task<PaymentLinkInformation> GetPaymentLinkInformation(int orderId)
        {
            return await _payOS.getPaymentLinkInformation(orderId);
        }

        // Method to cancel a payment link
        public async Task<PaymentLinkInformation> CancelPaymentLink(int paymentId)
        {
            return await _payOS.cancelPaymentLink(paymentId);
        }

        // Method to confirm a webhook URL
        public async Task<string> ConfirmWebhook(string webhookUrl)
        {
            return await _payOS.confirmWebhook(webhookUrl);
        }

        // Method to verify the payment webhook data
        public WebhookData VerifyPaymentWebhookData(WebhookType webhookBody)
        {
            return _payOS.verifyPaymentWebhookData(webhookBody);
        }
    }
}
