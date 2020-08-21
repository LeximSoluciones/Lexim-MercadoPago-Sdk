using System;
using System.Collections.Generic;
using MercadoPago.Common;
using MercadoPago.DataStructures.PaymentMethod;

namespace MercadoPago.Resources
{
    public sealed class PaymentMethod : Resource<PaymentMethod>
    {
        #region Actions

        /// <summary>
        /// Get All Payment Methods available
        /// </summary>
        public static List<PaymentMethod> All(Sites site, bool useCache = false, string accessToken = null) =>
            GetList($"/sites/{site}/payment_methods", accessToken, useCache);

        #endregion

        #region Properties

        /// <summary>
        /// Payment method identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name of the payment method
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Types of payment methods
        /// </summary>
        public PaymentTypeId PaymentTypeId { get; set; }

        /// <summary>
        /// Payment methods status
        /// </summary>
        public PaymentMethodStatus Status { get; set; }

        /// <summary>
        /// Logo to display on secure sites
        /// </summary>
        public string SecureThumbnail { get; set; }

        /// <summary>
        /// Logo to show
        /// </summary>
        public string Thumbnail { get; set; }

        /// <summary>
        /// Whether the capture can be delayed or not
        /// </summary> 
        public PaymentMethodDeferredCapture DeferredCapture { get; set; }

        /// <summary>
        /// Payment method settings
        /// </summary> 
        public List<Settings> Settings { get; set; }

        /// <summary>
        /// List of additional information that must be supplied by the payer
        /// </summary> 
        public List<String> AdditionalInfoNeeded { get; set; }

        /// <summary>
        /// Minimum amount that can be processed with this payment method
        /// </summary> 
        public string MinAllowedAmount { get; set; }

        /// <summary>
        /// Maximum amount that can be processed with this payment method
        /// </summary> 
        public string MaxAllowedAmount { get; set; }

        /// <summary>
        /// How many time in minutes could happen until processing of the payment
        /// </summary> 
        public string AccreditationTime { get; set; }

        /// <summary>
        /// Finantial institution processing this payment method
        /// </summary> 
        public List<String> FinancialInstitutions { get; set; }

        public List<string> ProcessingMode { get; set; }

        #endregion
    }
}
