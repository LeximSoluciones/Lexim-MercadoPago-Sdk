using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MercadoPago.DataStructures.MerchantOrder;

namespace MercadoPago.Resources
{
    public sealed partial class MerchantOrder : Resource<MerchantOrder>
    {
        #region Actions

        public static MerchantOrder FindById(string id, bool useCache = false, string accessToken = null) => 
            Get($"/merchant_orders/{id}", accessToken, useCache);

        public MerchantOrder Load(string id, bool useCache = false, string accessToken = null) => 
            Get($"/merchant_orders/{id}", accessToken, useCache);

        public MerchantOrder Save() => Post("/merchant_orders");

        public MerchantOrder Update() => Put($"/merchant_orders/{Id}");

        #endregion

        #region Properties

        public string Id { get; set; }

        [Required]
        public string PreferenceId { get; set; }

        public DateTime? DateCreated { get; private set; }

        public DateTime? LastUpdate { get; private set; }

        public string ApplicationId { get; set; }

        public string Status { get; private set; }

        public string SiteId { get; set; }

        public Payer Payer { get; set; }

        public Collector? Collector { get; set; }

        public long? SponsorId { get; set; }

        public List<MerchantOrderPayment> Payments { get; private set; }

        public decimal? PaidAmount { get; private set; }

        public decimal? RefundedAmount { get; private set; }

        public decimal? ShippingCost { get; private set; }

        public bool? Cancelled { get; set; }

        public List<Item> Items { get; set; }

        public List<Shipment> Shipments { get; set; }

        [StringLength(500)]
        public string NotificationUrl { get; set; }

        [StringLength(600)]
        public string AdditionalInfo { get; set; }

        [StringLength(256)]
        public string ExternalReference { get; set; }

        [StringLength(256)]
        public string Marketplace { get; set; }

        public decimal? TotalAmount { get; private set; }

        #endregion

        public void AppendShipment(Shipment shipment)
        {
            if (Shipments == null)
            {
                Shipments = new List<Shipment>();
            }
            Shipments.Add(shipment);
        }

        public void AppendItem(Item item)
        {
            if (Items == null)
            {
                Items = new List<Item>();
            }
            Items.Add(item);
        }
    }
}
