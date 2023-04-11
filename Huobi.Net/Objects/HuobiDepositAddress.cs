using CryptoExchange39.Net.Attributes;
using CryptoExchange39.Net.Converters;
using Huobi.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Globalization;
using CryptoExchange39.Net.ExchangeInterfaces;

namespace Huobi.Net.Objects
{
    /// <summary>
    /// Deposit address info
    /// </summary>
    public class HuobiDepositAddress 
    {
        /// <summary>
        /// Crypto currency
        /// </summary>
        public string Currency { get; set; } = "";
        /// <summary>
        /// Deposit address
        /// </summary>
        public string Address { get; set; } = "";
        /// <summary>
        /// Deposit address tag
        /// </summary>
        public string AddressTag { get; set; } = "";
        /// <summary>
        /// Block chain name
        /// </summary>
        public string Chain { get; set; } = "";
    }
}
