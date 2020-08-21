using System.Collections.Generic;
using System.Linq;

namespace MercadoPago.Resources
{
    public class DiscountCampaign : Resource<DiscountCampaign>
    {
        #region Actions

        public static DiscountCampaign Find(decimal transactionAmount, string payerEmail, string couponCode = null, bool useCache = false, MPRequestOptions requestOptions = null)
        {
            var query = CreateQuery("/v1/discount_campaigns");

            query = query.Where(x => x.TransactionAmount == transactionAmount)
                         .Where(x => x.PayerEmail == payerEmail)
                         .Where(x => x.CouponCode == couponCode);

            return query.ToList().FirstOrDefault();
        }

        #endregion

        #region Properties

        public string Id { get; set; }

        public string Name { get; set; }

        public decimal? PercentOff { get; set; }

        public decimal? AmountOff { get; set; }

        public decimal? CouponAmount { get; set; }

        public string CurrencyId { get; set; }

        public decimal? TransactionAmount { get; private set; }
        public string PayerEmail { get; private set; }
        public string CouponCode { get; private set; }

        #endregion
    }
}