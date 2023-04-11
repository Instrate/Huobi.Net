﻿using System;
using System.Collections.Generic;
using CryptoExchange39.Net.ExchangeInterfaces;
using CryptoExchange39.Net.Interfaces;
using Newtonsoft.Json;

namespace Huobi.Net.Objects
{
    /// <summary>
    /// Order book
    /// </summary>
    public class HuobiOrderBook: ICommonOrderBook
    {
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonIgnore]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// List of bids
        /// </summary>
        public IEnumerable<HuobiOrderBookEntry> Bids { get; set; } = new List<HuobiOrderBookEntry>();
        /// <summary>
        /// List of asks
        /// </summary>
        public IEnumerable<HuobiOrderBookEntry> Asks { get; set; } = new List<HuobiOrderBookEntry>();

        IEnumerable<ISymbolOrderBookEntry> ICommonOrderBook.CommonBids => Bids;
        IEnumerable<ISymbolOrderBookEntry> ICommonOrderBook.CommonAsks => Asks;
    }
}
