using System;
using System.Collections.Generic;
using System.Text;
using CryptoExchange39.Net.Interfaces;

namespace CryptoExchange39.Net.ExchangeInterfaces
{
    /// <summary>
    /// Common order book
    /// </summary>
    public interface ICommonOrderBook
    {
        /// <summary>
        /// Bids
        /// </summary>
        IEnumerable<ISymbolOrderBookEntry> CommonBids { get; }
        /// <summary>
        /// Asks
        /// </summary>
        IEnumerable<ISymbolOrderBookEntry> CommonAsks { get; }
    }
}
