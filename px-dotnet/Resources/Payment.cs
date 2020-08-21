﻿using MercadoPago.DataStructures.Payment;
using MercadoPago.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Converters;
using MercadoPago.DataStructures.Customer;

namespace MercadoPago.Resources
{
    /// <summary>
    /// This service allows you to create, modify or read payments
    /// </summary>
    public sealed partial class Payment : Resource<Payment>
    {
        #region Actions

        /// <summary>
        /// Find a payment through an unique identifier with Local Cache Flag
        /// </summary>
        public static Payment FindById(long? id, bool useCache = false, string accessToken = null) => 
            Get($"/v1/payments/{id}", accessToken, useCache);

        /// <summary>
        /// Save a new payment
        /// </summary>
        public Payment Save() => Post("/v1/payments");
        /// <summary>
        /// Update editable properties
        /// </summary>
        public Payment Update() => Put($"/v1/payments/{Id}");

        /// <summary>
        /// Get all payments, with using cache option
        /// </summary>
        public static List<Payment> All(bool useCache = false, string accessToken = null) => GetList("/v1/payments/search", accessToken, useCache);
        
        /// <summary>
        /// Get all payments acoording to specific filters, with using cache option
        /// </summary>
        [Obsolete("string-based dictionary usage is discouraged. Please use the Query() method and LINQ instead.")]
        public static List<Payment> Search(Dictionary<string, string> filters, bool useCache = false, string accessToken = null) => 
            GetList("/v1/payments/search", accessToken, useCache, filters);

        public static IQueryable<Payment> Query(bool useCache = false, string accessToken = null) =>
            CreateQuery("/v1/payments/search", accessToken, useCache);

        #endregion

        #region Interactions
        /// <summary>
        /// Payment refund
        /// </summary> 
        public Payment Refund()
        {
            var refund = new Refund
            {
                PaymentId = this.Id
            };
            refund.Save();

            if (refund.Id.HasValue)
            {
                this.Status = PaymentStatus.refunded;
            }
            else
            {
                //this.DelegateErrors(refund.Errors);
            }
            return this;
        }

        /// <summary>
        /// Partial payment refund
        /// </summary> 
        public Payment Refund(decimal amount)
        {
            var refund = new Refund
            {
                PaymentId = this.Id,
                Amount = amount
            };

            refund.Save();

            if (refund.Id.HasValue)
            {
                var payment = Payment.FindById(Id);
                Status = payment.Status;
                StatusDetail = payment.StatusDetail;
                TransactionAmountRefunded = payment.TransactionAmountRefunded;
                Refunds = payment.Refunds;
            }
            else
            {
                //Errors = refund.Errors;
            }

            return this;
        }

        #endregion

        #region Properties 

        /// <summary>
        /// Payment identifier
        /// </summary>
        public long? Id { get; private set; }
        /// <summary>
        /// Payment’s creation date
        /// </summary>
        public DateTime? DateCreated { get; private set; }
        /// <summary>
        /// Payment’s approval date
        /// </summary>
        public DateTime? DateApproved { get; private set; }

        /// <summary>
        /// Last modified date
        /// </summary>
        public DateTime? DateLastUpdated { get; private set; }

        /// <summary>
        /// Release date of payment
        /// </summary>
        public DateTime? MoneyReleaseDate { get; private set; }

        /// <summary>
        /// Identifies the seller
        /// </summary>
        public int? CollectorId { get; private set; }

        /// <summary>
        /// Payment type
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public OperationType? OperationType { get; private set; }

        /// <summary>
        /// Identifies the buyer  
        /// </summary>
        public Payer Payer { get; set; }

        /// <summary>
        /// When set to true, the payment can only be approved or rejected. 
        /// Otherwise in_process status is added
        /// </summary>
        public bool? BinaryMode { get; set; }

        /// <summary>
        /// Whether the payment will be processed in sandbox or in production mode
        /// </summary>
        public bool? LiveMode { get; private set; }

        /// <summary>
        /// Order identifier
        /// </summary>
        public Order? Order { get; set; }

        /// <summary>
        /// ID given by the merchant in their system
        /// </summary>
        public string ExternalReference { get; set; }

        /// <summary>
        /// Payment reason or item title
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Valid JSON that can be attached to the payment to record additional attributes of the merchant
        /// </summary>
        public JObject Metadata { get; set; }

        /// <summary>
        /// ID of the currency used in the payment
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public CurrencyId? CurrencyId { get; private set; }

        /// <summary>
        /// Product cost
        /// </summary>
        public decimal? TransactionAmount { get; set; }

        /// <summary>
        /// Total refunded amount in this payment
        /// </summary>
        public decimal? TransactionAmountRefunded { get; private set; }

        /// <summary>
        /// Amount of the coupon discount
        /// </summary>
        public decimal? CouponAmount { get; set; }

        /// <summary>
        /// Discount campaign ID
        /// </summary>
        public int? CampaignId { get; set; }

        /// <summary>
        /// Discount campaign with a specific code
        /// </summary>
        public string CouponCode { get; set; }

        /// <summary>
        /// Groups the details of the transaction
        /// </summary>
        public TransactionDetail? TransactionDetails { get; private set; }

        /// <summary>
        /// List of fees
        /// </summary>
        public List<FeeDetail> FeeDetails { get; private set; }

        /// <summary>
        /// Id of the scheme for the absorption of financing fee
        /// </summary>
        public int? DifferentialPricingId { get; set; }

        /// <summary>
        /// Fee collected by a marketplace or MercadoPago Application
        /// </summary>
        public decimal? ApplicationFee { get; set; }

        /// <summary>
        /// Payment status
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentStatus? Status { get; set; }

        /// <summary>
        /// Gives more detailed information on the current state or rejection cause
        /// </summary>
        public string StatusDetail { get; private set; }

        /// <summary>
        /// Gives more detailed information on the current state or rejection cause
        /// </summary>
        public bool? Capture { get; set; }

        /// <summary>
        /// Gives more detailed information on the current state or rejection cause
        /// </summary>
        public bool? Captured { get; private set; }

        /// <summary>
        /// Gives more detailed information on the current state or rejection cause
        /// </summary>
        public string CallForAuthorizeId { get; private set; }

        /// <summary>
        /// Gives more detailed information on the current state or rejection cause
        /// </summary>
        [Required]
        public string PaymentMethodId { get; set; }

        /// <summary>
        /// Payment method issuer
        /// </summary>
        public string IssuerId { get; set; }

        /// <summary>
        /// Type of payment method chosen
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentTypeId? PaymentTypeId { get; private set; }

        /// <summary>
        /// Card token ID
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Details of the card used
        /// </summary>
        public DataStructures.Payment.Card? Card { get; private set; }

        /// <summary>
        /// How will look the payment in the card bill (e.g.: MERCADOPAGO)
        /// </summary>
        public string StatementDescriptor { get; set; }

        /// <summary>
        /// Selected quantity of installments
        /// </summary>
        [Range(1,99)]
        public int Installments { get; set; }

        /// <summary>
        /// URL where mercadopago will send notifications associated to changes in this payment
        /// </summary>
        public string NotificationUrl { get; set; }

        /// <summary>
        /// URL where mercadopago does the final redirect (only for bank transfers)
        /// </summary>
        public string CallbackUrl { get; set; }

        /// <summary>
        /// List of refunds that were made to this payment
        /// </summary>
        public List<Refund> Refunds { get; private set; }

        /// <summary>
        /// Data that could improve fraud analysis and conversion rates. 
        /// Try to send as much information as possible.
        /// </summary>
        public AdditionalInfo? AdditionalInfo { get; set; }

        /// <summary>
        /// Processing mode to define if an specific merchannt id should be used.
        /// </summary>
        public string ProcessingMode { get; set; }

        /// <summary>
        /// Merchant Id for complex payment cases
        /// </summary>
        public string MerchantAccountId { get; set; }

        public DateTime? DateOfExpiration { get; set; }

        /// <summary>
        /// Sponsor Identification
        /// </summary>
        public long? SponsorId { get; set; }

        /// <summary>
        /// Taxes for payments
        /// </summary>
        public List<Taxes> Taxes { get; set; }
        #endregion

    }
}
