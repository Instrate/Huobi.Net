using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Huobi.Net.Objects
{
    /// <summary>
    /// Usdt swap position info
    /// </summary>
    public class UsdtSwapPositionInfo
    {
        public string? Symbol { get; set; }
        [JsonProperty("contract_code")]
        public string? ContractCode { get; set; }
        public decimal? Volume { get; set; }
        public decimal? Available { get; set; }
        public decimal? Frozen { get; set; }
        [JsonProperty("cost_open")]
        public decimal? CostOpen { get; set; }
        [JsonProperty("cost_hold")]
        public decimal? CostHold { get; set; }
        [JsonProperty("profit_unreal")]
        public decimal? ProfitUnreal { get; set; }
        [JsonProperty("profit_rate")]
        public decimal? ProfitRate { get; set; }
        [JsonProperty("lever_rate")]
        public int? LeverRate { get; set; }
        [JsonProperty("position_margin")]
        public decimal? PositionMargin { get; set; }
        public string? Direction { get; set; }
        public decimal? Profit { get; set; }
       [ JsonProperty("last_price")]
        public decimal? LastPrice { get; set; }
        [JsonProperty("margin_asset")]
        public string? MarginAsset { get; set; }
        [JsonProperty("margin_mode")]
        public string? MarginMode { get; set; }
        [JsonProperty("margin_account")]
        public string? MarginAccount { get; set; }
    }
    /*
     "data": [
        {
            "symbol": "BTC",
            "contract_code": "BTC-USDT",
            "volume": 2,
            "available": 2,
            "frozen": 0,
            "cost_open": 51179.1,
            "cost_hold": 51179.1,
            "profit_unreal": 0,
            "profit_rate": 0,
            "lever_rate": 100,
            "position_margin": 10.23582,
            "direction": "sell",
            "profit": 0,
            "last_price": 51179.1,
            "margin_asset": "USDT",
            "margin_mode": "cross",
            "margin_account": "USDT"
        }
    */
}
