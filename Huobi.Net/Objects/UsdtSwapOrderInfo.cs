using Huobi.Net.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Huobi.Net.Objects
{
    /// <summary>
    /// usdt swap order info
    /// </summary>
    public class UsdtSwapOrderInfo
    {
        public string? Symbol { get; set; }
        [JsonProperty("contract_code")]
        public string? ContractCode { get; set; }
        public decimal? Volume { get; set; }
        public decimal? Price { get; set; }
        [JsonProperty("order_price_type")]
        public EUsdtSwapOrderPriceType? OrderPriceType { get; set; }
        [JsonProperty("order_type")]
        public int? OrderType { get; set; }
        public EUsdtSwapOrderDirection? Direction { get; set; }
        public EUsdtSwapOrderOffset? Offset { get; set; }
        [JsonProperty("lever_rate")]
        public int? LeverRate { get; set; }
        [JsonProperty("order_id")]
        public long? OrderId { get; set; }
        [JsonProperty("client_order_id")]
        public long? ClientOrderId { get; set; }
        [JsonProperty("created_at")]
        public long? CreatedAt { get; set; }
        [JsonProperty("trade_volume")]
        public decimal? TradeVolume { get; set; }
        [JsonProperty("trade_turnover")]
        public decimal? TradeTurnover { get; set; }
        public decimal? Fee { get; set; }
        [JsonProperty("trade_avg_price")]
        public decimal? TradeAvgPrice { get; set; }
        [JsonProperty("margin_frozen")]
        public decimal? MarginFrozen { get; set; }
        public decimal? Profit { get; set; }
        public decimal? Status { get; set; }
        [JsonProperty("order_source")]
        public string? OrderSource { get; set; }
        [JsonProperty("order_id_str")]
        public string? OrderIdString { get; set; }
        [JsonProperty("fee_asset")]
        public string? FeeAsset { get; set; }
        [JsonProperty("liquidation_type")]
        public string? LiquidationType { get; set; }
        [JsonProperty("canceled_at")]
        public long? CanceledAt { get; set; }
        [JsonProperty("margin_asset")]
        public string? MarginAsset { get; set; }
        [JsonProperty("margin_account")]
        public string? MarginAccount { get; set; }
        [JsonProperty("margin_mode")]
        public string? MarginMode { get; set; }
        [JsonProperty("is_tpsl")]
        public int? IsTpSl { get; set; }
        [JsonProperty("real_profit")]
        public decimal? RealProfit { get; set; }
    }
}
