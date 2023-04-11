﻿using System;
using System.Collections.Generic;
using System.Text;
using CryptoExchange39.Net.ExchangeInterfaces;

namespace Huobi.Net.Objects
{
    /// <summary>
    /// Placed order
    /// </summary>
    public class HuobiPlacedOrder : ICommonOrderId
    {
        /// <summary>
        /// The id
        /// </summary>
        public long Id { get; set; }
        string ICommonOrderId.CommonId => Id.ToString();
    }
}
