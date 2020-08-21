using MercadoPago.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace MercadoPago.Resources
{
    public sealed partial class AuthorizedPayment : Resource<AuthorizedPayment>
    {
        #region Actions

        /// <summary>
        /// Get all authorized payments acoording to specific filters
        /// </summary>
        public static List<AuthorizedPayment> Search(Dictionary<string, string> filters, bool useCache = false, string accessToken = null) =>
            GetList("/authorized_payments/search", accessToken, useCache, filters);

        public static IQueryable<AuthorizedPayment> Query(bool useCache = false, string accessToken = null) =>
            CreateQuery("/authorized_payments/search", accessToken, useCache);

        /// <summary>
        /// Find an authorized payment trought an unique identifier with Local Cache Flag
        /// </summary>
        public static AuthorizedPayment FindById(long? id, bool useCache = false, string accessToken = null) =>
            Get($"/authorized_payments/{id}", accessToken, useCache);

        /// <summary>
        /// Save an authorized payment
        /// </summary>
        public AuthorizedPayment Save() => Post("/authorized_payments");

        /// <summary>
        ///  Update editable properties
        /// </summary>
        public AuthorizedPayment Update() => Put($"/authorized_payments/{Id}");

        #endregion

        #region Properties

        /// <summary>
        ///  Authorized payment ID
        /// </summary>
        public long? Id { get; private set; }

        /// <summary>
        ///  Preapproval ID
        /// </summary>
        public string PreapprovalId { get; set; }

        /// <summary>
        /// Processings way of this payment
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public AuthorizedPaymentType? Type { get; private set; }

        /// <summary>
        /// Current processing status
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public AuthorizedPaymentStatus? Status { get; private set; }

        /// <summary>
        /// Creation date of this entity
        /// </summary>
        public DateTime DateCreated { get; private set; }

        /// <summary>
        /// Last modified date
        /// </summary>
        public DateTime LastModified { get; private set; }

        /// <summary>
        /// Price of the purchased item or service
        /// </summary>
        public decimal? TransactionAmount { get; set; }

        /// <summary>
        /// Currency ID used in this payment
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public CurrencyId? CurrencyId { get; set; }

        /// <summary>
        /// Reason for payment
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// ID of the associated purchase order
        /// </summary>
        [StringLength(256)]
        public string ExternalReference { get; set; }

        /// <summary>
        /// Data of the associated payment
        /// </summary>
        public MercadoPago.DataStructures.AuthorizedPayment.Payment Payment { get; private set; }

        /// <summary>
        /// Original status_detail of the rejection, regardless the actual processing status (only returned if applies)
        /// </summary>
        public string RejectionCode { get; private set; }

        /// <summary>
        /// Current retry quantity
        /// </summary>
        public int RetryAttempt { get; private set; }

        /// <summary>
        /// Date for next retry attempt
        /// </summary>
        public DateTime NextRetryDate { get; private set; }

        /// <summary>
        /// Date of last retry attempt
        /// </summary>
        public DateTime LastRetryDate { get; private set; }

        /// <summary>
        /// Date in which the payment wouldnt be retried any more
        /// </summary>
        public DateTime ExpireDate { get; private set; }

        /// <summary>
        /// Original date of payment
        /// </summary>
        public DateTime DebitDate { get; private set; }

        /// <summary>
        /// Discount coupon code
        /// </summary>
        public string CouponCode { get; set; }

        #endregion
    }
}
