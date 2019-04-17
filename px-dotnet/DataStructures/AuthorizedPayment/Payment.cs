using System;
using System.Collections.Generic;
using System.Text;

namespace MercadoPago.DataStructures.AuthorizedPayment
{
    public class Payment
    {
        #region Properties

        /// <summary>
        /// Payment ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Payment status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Further detail of the current status
        /// </summary>
        public string StatusDetail { get; set; }

        #endregion
    }
}
