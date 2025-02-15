﻿using CryptoExchange39.Net.Attributes;
using CryptoExchange39.Net.Converters;
using Huobi.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Globalization;
using CryptoExchange39.Net.ExchangeInterfaces;

namespace Huobi.Net.Objects
{
    /// <summary>
    /// Order info
    /// </summary>
    public class HuobiOrder: ICommonOrder
    {
        /// <summary>
        /// The id of the order
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }

        /// <summary>
        /// The order id as specified by the client
        /// </summary>
        [JsonProperty("client-order-id")]
        public string ClientOrderId { get; set; } = "";

        /// <summary>
        /// The symbol of the order
        /// </summary>
        public string Symbol { get; set; } = "";
        /// <summary>
        /// The id of the account that placed the order
        /// </summary>
        [JsonProperty("account-id")]
        public long AccountId { get; set; }
        /// <summary>
        /// The amount of the order
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// The price of the order
        /// </summary>
        [JsonProperty("price")]
        public decimal Price { get; set; }
        
        /// <summary>
        /// The time the order was created
        /// </summary>
        [JsonProperty("created-at"), JsonConverter(typeof(TimestampConverter))]
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// The time the order was canceled
        /// </summary>
        [JsonProperty("canceled-at"), JsonConverter(typeof(TimestampConverter))]
        public DateTime CanceledAt { get; set; }
        /// <summary>
        /// The time the order was finished
        /// </summary>
        [JsonProperty("finished-at"), JsonConverter(typeof(TimestampConverter))]
        public DateTime FinishedAt { get; set; }

        /// <summary>
        /// The type of the order
        /// </summary>
        [JsonProperty("type"), JsonConverter(typeof(OrderTypeConverter))]
        public HuobiOrderType Type { get; set; }

        /// <summary>
        /// The source of the order
        /// </summary>
        [JsonProperty("source"), JsonOptionalProperty]
        public string Source { get; set; } = "";

        /// <summary>
        /// The state of the order
        /// </summary>
        [JsonProperty("state"), JsonConverter(typeof(OrderStateConverter))]
        public HuobiOrderState State { get; set; }

        /// <summary>
        /// The amount of the order that is filled
        /// </summary>
        [JsonProperty("field-amount")]
        public decimal FilledAmount { get; set; }

        /// <summary>
        /// Filled cash amount
        /// </summary>
        [JsonProperty("field-cash-amount")]
        public decimal FilledCashAmount { get; set; }

        /// <summary>
        /// The amount of fees paid for the filled amount
        /// </summary>
        [JsonProperty("field-fees")]
        public decimal FilledFees { get; set; }

        string ICommonOrderId.CommonId => Id.ToString(CultureInfo.InvariantCulture);
        string ICommonOrder.CommonSymbol => Symbol;
        decimal ICommonOrder.CommonPrice => Price;
        decimal ICommonOrder.CommonQuantity => Amount;
        string ICommonOrder.CommonStatus => State.ToString();
        bool ICommonOrder.IsActive =>
            State == HuobiOrderState.Created ||
            State == HuobiOrderState.PreSubmitted ||
            State == HuobiOrderState.Submitted ||
            State == HuobiOrderState.PartiallyFilled;

        IExchangeClient.OrderSide ICommonOrder.CommonSide => Type.ToString().ToLowerInvariant().Contains("buy")
            ? IExchangeClient.OrderSide.Buy
            : IExchangeClient.OrderSide.Sell;

        IExchangeClient.OrderType ICommonOrder.CommonType
        {
            get
            {
                if (Type == HuobiOrderType.LimitBuy
                    || Type == HuobiOrderType.LimitSell)
                    return IExchangeClient.OrderType.Limit;
                if (Type == HuobiOrderType.MarketBuy
                    || Type == HuobiOrderType.MarketSell)
                    return IExchangeClient.OrderType.Market;
                return IExchangeClient.OrderType.Other;
            }
        }
    }
}
