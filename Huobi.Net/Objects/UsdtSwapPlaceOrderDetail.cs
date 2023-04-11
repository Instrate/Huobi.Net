using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Huobi.Net.Objects
{
    /// <summary>
    /// Swap order place information
    /// </summary>
    public class UsdtSwapPlaceOrderDetail
    {
        /// <summary>
        /// Long order id
        /// </summary>
        [JsonProperty("order_id")]
        public long? OrderId { get; set; }
        /// <summary>
        /// String order id
        /// </summary>
        [JsonProperty("order_id_str")]
        public string? OrderIdStr { get; set; }
    }
}
