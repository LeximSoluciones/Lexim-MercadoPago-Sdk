using System;
using MercadoPago.Resources;

namespace MercadoPago
{
    public static class Ipn
    {
        internal const string Payment = "payment";
        internal const string MerchantOrder = "merchant_order";        
        internal const string AuthorizedPayment = "authorized_payment";
        internal const string Preapproval = "preapproval";

        public static void HandleNotification(string topic, string id, Action<Payment> onPaymentReceived = null, Action<MerchantOrder> onMerchantOrderReceived = null, Action<AuthorizedPayment> onAuthorizedPaymentReceived = null, Action<Preapproval> onPreapprovalReceived = null)
        {
            if (string.IsNullOrEmpty(topic) || string.IsNullOrEmpty(id))
            {
                throw new MPException("Topic and Id can not be null in the IPN request.");
            }

            if (onPaymentReceived == null && onMerchantOrderReceived == null && onAuthorizedPaymentReceived == null && onPreapprovalReceived == null)
                throw new ArgumentNullException($"{nameof(onPaymentReceived)} and {nameof(onMerchantOrderReceived)} and {nameof(onAuthorizedPaymentReceived)} and {nameof(onPreapprovalReceived)} cannot be null at the same time. Please specify at least one of them.");

            switch (topic)
            {                
                case Payment:
                    if (onPaymentReceived != null)
                    {
                        if (!long.TryParse(id, out var paymentId))
                            throw new MPException($"Invalid Payment Id: {id}");
                        var payment = Resources.Payment.FindById(paymentId);
                        onPaymentReceived(payment);
                    }
                    break;
                case MerchantOrder:
                    if (onMerchantOrderReceived != null)
                    {
                        var merchantOrder = Resources.MerchantOrder.FindById(id);
                        onMerchantOrderReceived(merchantOrder);
                    }
                    break;
                case AuthorizedPayment:
                    if (onAuthorizedPaymentReceived != null)
                    {
                        if (!long.TryParse(id, out var authorizedPaymentId))
                            throw new MPException($"Invalid Authorized Payment Id: {id}");
                        var authorizedPayment = Resources.AuthorizedPayment.FindById(authorizedPaymentId);
                        onAuthorizedPaymentReceived(authorizedPayment);
                    }
                    break;
                case Preapproval:
                    if (onPreapprovalReceived != null)
                    {
                        var preapproval = Resources.Preapproval.FindById(id);
                        onPreapprovalReceived(preapproval);
                    }
                    break;
                default:
                    throw new MPException($"IPN Notification with topic '{topic}' cannot be handled. Please create an issue at https://github.com/LeximSoluciones/Lexim-MercadoPago-Sdk/issues if you use this notification type.");
            }
        }
    }
}