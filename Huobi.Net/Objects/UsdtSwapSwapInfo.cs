using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Huobi.Net.Objects
{
    /// <summary>
    /// Swap contract information
    /// </summary>
    public class UsdtSwapSwapInfo
    {
        /// <summary>
        /// Symbol
        /// </summary>
        public string? Symbol { get; set; }
        /// <summary>
        /// Contract Code
        /// </summary>
        [JsonProperty("contract_code")]
        public string? ContractCode { get; set; }
        /// <summary>
        /// Contract Value (USDT of one contract)
        /// </summary>
        [JsonProperty("contract_size")]
        public decimal? ContractSize { get; set; }
        /// <summary>
        /// Minimum Variation of Contract Price
        /// </summary>
        [JsonProperty("price_tick")]
        public decimal? PriceTick { get; set; }
        /// <summary>
        /// Listing Date
        /// </summary>
        [JsonProperty("create_date")]
        public string? CreateDate { get; set; }
        /// <summary>
        /// delivery time（When the contract does not need to be delivered, 
        /// the field value is an empty string），millesecond timestamp
        /// </summary>
        [JsonProperty("delivery_time")]
        public string? DeliveryTime { get; set; }
        /// <summary>
        /// Contract Status
        /// </summary>
        [JsonProperty("contract_status")]
        public int? ContractStatus { get; set; }
        /// <summary>
        /// Settlement Date
        /// </summary>
        [JsonProperty("settlement_date")]
        public string? SettlementDate { get; set; }
        /// <summary>
        /// support margin mode
        /// </summary>
        [JsonProperty("support_margin_mode")]
        public string? SupportMarginMode { get; set; }

    }
}
