﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoExchange39.Net.ExchangeInterfaces
{
    /// <summary>
    /// Common symbol
    /// </summary>
    public interface ICommonSymbol
    {
        /// <summary>
        /// Symbol name
        /// </summary>
        public string CommonName { get; }
        /// <summary>
        /// Minimum trade size
        /// </summary>
        public decimal CommonMinimumTradeSize { get; }
    }
}
