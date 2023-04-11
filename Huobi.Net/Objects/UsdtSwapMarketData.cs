using System;
using System.Collections.Generic;
using CryptoExchange39.Net.Converters;
using CryptoExchange39.Net.ExchangeInterfaces;
using Newtonsoft.Json;

namespace Huobi.Net.Objects
{
    /// <summary>
    /// Contract data for usdt swap market
    /// </summary>
    public class UsdtSwapMarketData
    {
        /// <summary>
        /// kline id,the same as kline timestamp
        /// </summary>
        [JsonProperty("id")]
        public long? Id { get; set; }
        /// <summary>
        /// Trade Volume(Cont.), from nowtime - 24 hours. Sum of both buy and sell sides
        /// </summary>
        [JsonProperty("vol")]
        public string? Volume { get; set; }
        /// <summary>
        /// Contract Quantity, from nowtime - 24 hours. Sum of both buy and sell sides
        /// </summary>
        [JsonProperty("count")]
        public decimal? Count { get; set; }

        /// <summary>
        /// Open
        /// </summary>
        [JsonProperty("open")]
        public string? Open { get; set; }
        /// <summary>
        /// High
        /// </summary>
        [JsonProperty("high")]
        public string? High { get; set; }
        /// <summary>
        /// Low
        /// </summary>
        [JsonProperty("low")]
        public string? Low { get; set; }
        /// <summary>
        /// Close
        /// </summary>
        [JsonProperty("close")]
        public string? Close { get; set; }
        /// <summary>
        /// Trade Amount(Coin), trade amount(coin)=
        /// sum(order quantity of a single order * face value of the coin/order price),
        /// from nowtime - 24 hours. Sum of both buy and sell sides
        /// </summary>
        [JsonProperty("amount")]
        public string? Amount { get; set; }

        /// <summary>
        /// Sell,[price(Ask price), vol(Ask orders (cont.) )], price in ascending sequence
        /// </summary>
        [JsonProperty("ask")]
        public List<decimal>? Ask { get; set; }
        /// <summary>
        /// Buy,[price(Bid price), vol(Bid orders(Cont.))], Price in descending sequence
        /// </summary>
        [JsonProperty("bid")]
        public List<decimal>? Bid { get; set; }
        /// <summary>
        /// Transaction amount, that is, 
        /// sum (transaction quantity * contract face value * transaction price),
        /// from nowtime - 24 hours. Sum of both buy and sell sides
        /// </summary>
        [JsonProperty("trade_turnover")]
        public string? TradeTurnOver { get; set; }
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonProperty("ts")]
        public long TimeStamp { get; set; }
    }
}
