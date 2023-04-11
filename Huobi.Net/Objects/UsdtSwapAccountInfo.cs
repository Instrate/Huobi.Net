using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Huobi.Net.Objects
{
    /// <summary>
    /// Usdt-M Account info 
    /// </summary>
    public class UsdtSwapAccountInfo
    {
        [JsonProperty("margin_mode")]
        public string MarginMode { get; set; }
        [JsonProperty("margin_account")]
        public string MarginAccount { get; set; }
        [JsonProperty("margin_asset")]
        public string MarginAsset { get; set; }
        [JsonProperty("margin_balance")]
        public decimal? MarginBalance { get; set; }
        [JsonProperty("margin_static")]
        public decimal? MarginStatic { get; set; }
        [JsonProperty("margin_position")]
        public decimal? MarginPosition { get; set; }
        [JsonProperty("margin_frozen")]
        public decimal? MarginFrozen { get; set; }
        [JsonProperty("profit_real")]
        public decimal? ProfitReal { get; set; }
        [JsonProperty("profit_unreal")]
        public decimal? ProfitUnreal { get; set; }
        [JsonProperty("withdraw_available")]
        public decimal? WithdrawAvailable { get; set; }
        [JsonProperty("risk_rate")]
        public decimal? RiskRate { get; set; }
        [JsonProperty("contract_detail")]
        public List<UsdtSwapAccountContractDetail> ContractDetail { get; set; }
    }

    /// <summary>
    /// Contract detail in account info
    /// </summary>
    public class UsdtSwapAccountContractDetail
    {
        public string Symbol { get; set; }
        [JsonProperty("contract_code")]
        public string ContractCode { get; set; }
        [JsonProperty("margin_position")]
        public decimal? MarginPosition { get; set; }
        [JsonProperty("margin_frozen")]
        public decimal? MarginFrozen { get; set; }
        [JsonProperty("margin_available")]
        public decimal? MarginAvailable { get; set; }
        [JsonProperty("profit_unreal")]
        public decimal? ProfitUnreal { get; set; }
        [JsonProperty("liquidation_price")]
        public decimal? LiquidationPrice { get; set; }
        [JsonProperty("lever_rate")]
        public decimal? Leverage { get; set; }
        [JsonProperty("adjust_factor")]
        public decimal? AdjustFactor { get; set; }
    }
    /*
      "data":[
        {
            "margin_mode":"cross",
            "margin_account":"USDT",
            "margin_asset":"USDT",
            "margin_balance":0.000000549410817836,
            "margin_static":0.000000549410817836,
            "margin_position":0,
            "margin_frozen":0,
            "profit_real":0,
            "profit_unreal":0,
            "withdraw_available":0.000000549410817836,
            "risk_rate":null,
            "contract_detail":[
                {
                    "symbol":"BTC",
                    "contract_code":"BTC-USDT",
                    "margin_position":0,
                    "margin_frozen":0,
                    "margin_available":0.000000549410817836,
                    "profit_unreal":0,
                    "liquidation_price":null,
                    "lever_rate":100,
                    "adjust_factor":0.55
                },
                {
                    "symbol":"EOS",
                    "contract_code":"EOS-USDT",
                    "margin_position":0,
                    "margin_frozen":0,
                    "margin_available":0.000000549410817836,
                    "profit_unreal":0,
                    "liquidation_price":null,
                    "lever_rate":5,
                    "adjust_factor":0.06
                }
            ]
        }
    ]
   */
}
