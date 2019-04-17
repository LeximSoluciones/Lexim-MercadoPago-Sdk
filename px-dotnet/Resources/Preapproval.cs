using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MercadoPago.Common;
using MercadoPago.DataStructures.Preapproval;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MercadoPago.Resources
{
    public sealed partial class Preapproval : Resource<Preapproval>
    {
        #region Actions

        /// <summary>
        /// Get all preapprovals acoording to specific filters
        /// </summary>
        public static List<Preapproval> Search(Dictionary<string, string> filters, bool useCache = false, string accessToken = null) =>
            GetList("/preapproval/search", accessToken, useCache, filters);

        public static IQueryable<Preapproval> Query(bool useCache = false, string accessToken = null) =>
            CreateQuery("/preapproval/search", accessToken, useCache);

        /// <summary>
        /// Find a preapproval trought an unique identifier with Local Cache Flag
        /// </summary>
        public static Preapproval FindById(string id, bool useCache = false, string accessToken = null) => 
            Get($"/preapproval/{id}", accessToken, useCache);

        /// <summary>
        /// Save a new preapproval
        /// </summary>
        public Preapproval Save() => Post("/preapproval");

        /// <summary>
        ///  Update editable properties
        /// </summary>
        public Preapproval Update() => Put($"/preapproval/{Id}");

        #endregion

        #region Properties

        /// <summary>
        ///  Preapproval ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///  User's ID issuer of payment
        /// </summary>
        public int? PayerId { get; set; }

        /// <summary>
        ///  User's email issuer of payment
        /// </summary>
        [StringLength(256)]
        public string PayerEmail { get; set; }

        /// <summary>
        ///  URL to redirect the user when the preapproval is authorized
        /// </summary>
        [StringLength(256)]
        public string BackUrl { get; set; }

        /// <summary>
        ///  User's ID of payee
        /// </summary>
        public int? CollectorId { get; set; }

        /// <summary>
        ///  Application ID of payee
        /// </summary>
        public int? ApplicationId { get; set; }

        /// <summary>
        /// Current preapproval status
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public PreapprovalStatus? Status { get; set; }

        /// <summary>
        /// Optional to enable automatic charging
        /// </summary>
        public AutoRecurring? AutoRecurring { get; set; }

        /// <summary>
        /// URL for accessing the authorization form
        /// </summary>
        [StringLength(256)]
        public string InitPoint { get; private set; }

        /// <summary>
        /// URL for accessing the authorization form in a sandbox environment (for testing purposes)
        /// </summary>
        [StringLength(256)]
        public string SandboxInitPoint { get; private set; }

        /// <summary>
        ///  Reason for authorization
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Reference you can synchronize with your payment system
        /// </summary>
        [StringLength(256)]
        public string ExternalReference { get; set; }

        /// <summary>
        /// Creation date of this entity
        /// </summary>
        public DateTime? DateCreated { get; private set; }

        /// <summary>
        /// Last modified date
        /// </summary>
        public DateTime? LastModified { get; private set; }

        /// <summary>
        ///  ID of the subscription plan from which this preapproval was created
        /// </summary>
        [StringLength(256)]
        public string PreapprovalPlanId { get; set; }

        #endregion
    }
}