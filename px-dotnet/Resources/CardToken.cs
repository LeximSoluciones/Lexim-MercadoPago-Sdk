using System;

namespace MercadoPago.Resources
{
    public class CardToken : Resource<CardToken>
    {
        #region Actions

        public CardToken Save(MPRequestOptions requestOptions = null) =>
            Post("/v1/card_tokens");

        public static CardToken FindById(string id, bool useCache, MPRequestOptions requestOptions) =>
            Get($"/v1/card_tokens/{id}");

        #endregion

        #region Properties  

        public string Id { get; set; }

        public string PublicKey { get; set; }

        public string CustomerId { get; set; }

        public string CardId { get; set; }

        public string Status { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateLastUpdate { get; set; }

        public DateTime? DateDue { get; set; }

        public bool? LuhnValidation { get; set; }

        public bool? LineMode { get; set; }

        public bool? RequireEsc { get; set; }

        public string SecurityCode { get; set; }

        #endregion
    }
}