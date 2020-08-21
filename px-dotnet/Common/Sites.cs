using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MercadoPago.Common
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Sites
    {
        /// <summary> Mercado Libre Argentina </summary>
        MLA,
        /// <summary> Mercado Livre Brasil </summary>
        MLB,
        /// <summary> Mercado Libre Chile </summary>
        MLC,
        /// <summary> Mercado Libre Uruguay </summary>
        MLU,
        /// <summary> Mercado Libre Colombia </summary>
        MCO,
        /// <summary> Mercado Libre Venezuela </summary>
        MLV,
        /// <summary> Mercado Libre Perú </summary>
        MPE,
        /// <summary> Mercado Libre México </summary>
        MLM
    }
}
