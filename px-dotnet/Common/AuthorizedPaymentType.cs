using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace MercadoPago.Common
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AuthorizedPaymentType
    {
        ///<summary>The payment is processed immediately when it is submitted</summary>
        online,
        ///<summary>The payment is scheduled for later processing, also supports failures recycling</summary>
        scheduled
    }
}
