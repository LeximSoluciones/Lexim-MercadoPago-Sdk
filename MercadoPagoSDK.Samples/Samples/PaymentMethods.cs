using MercadoPago.DataStructures.Payment;
using MercadoPago.Resources;
using System.Collections.Generic;

namespace MercadoPagoSDK.Samples
{
    internal class PaymentMethods : ISample, IRequiresAccessToken
    {
        public string Name => "Get Payment Methods";

        public string Category => "Payment Methods";

        public void Run()
        {
            List<PaymentMethod> list = PaymentMethod.All(MercadoPago.Common.Sites.MLA);   

        }
    }
}
