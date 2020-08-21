using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace MercadoPago.Common
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AuthorizedPaymentStatus
    {
        ///<summary>The payment was not processed yet, but is scheduled for later processing</summary>
        scheduled,
        ///<summary>The payment processing resulted in failure, will be retried later</summary>
        recycling,
        ///<summary>The payment was processed</summary>
        processed,
        ///<summary>The payment processing was cancelled by the seller</summary>
        cancelled
    }
}
