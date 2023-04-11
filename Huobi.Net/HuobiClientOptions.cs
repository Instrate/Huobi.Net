﻿using System.Net.Http;
using CryptoExchange39.Net.Objects;
using Huobi.Net.Interfaces;

namespace Huobi.Net
{
    /// <summary>
    /// Client options
    /// </summary>
    public class HuobiClientOptions: RestClientOptions
    {
        /// <summary>
        /// Whether public requests should be signed if ApiCredentials are provided. Needed for accurate rate limiting.
        /// </summary>
        public bool SignPublicRequests { get; set; } = false;

        /// <summary>
        /// Create new client options
        /// </summary>
        ///public HuobiClientOptions() : this(null, "https://api.huobi.pro")
        public HuobiClientOptions() : this(null, "https://api.hbdm.com")
        {
        }

        /// <summary>
        /// Create new client options
        /// </summary>
        /// <param name="client">HttpClient to use for requests from this client</param>
        ///public HuobiClientOptions(HttpClient client) : this(client, "https://api.huobi.pro")
        public HuobiClientOptions(HttpClient client) : this(client, "https://api.hbdm.com")
        {
        }

        /// <summary>
        /// Create new client options
        /// </summary>
        /// <param name="apiAddress">Custom API address to use</param>
        /// <param name="client">HttpClient to use for requests from this client</param>
        public HuobiClientOptions(HttpClient? client, string apiAddress) : base(apiAddress)
        {
            HttpClient = client;
        }

        /// <summary>
        /// Copy
        /// </summary>
        /// <returns></returns>
        public HuobiClientOptions Copy()
        {
            var copy = Copy<HuobiClientOptions>();
            copy.SignPublicRequests = SignPublicRequests;
            return copy;
        }
    }

    /// <summary>
    /// Socket client options
    /// </summary>
    public class HuobiSocketClientOptions : SocketClientOptions
    {
        /// <summary>
        /// The base address for the authenticated websocket
        /// </summary>
        public string BaseAddressAuthenticated { get; set; } = "wss://api.huobi.pro/ws/v2";

        /// <summary>
        /// ctor
        /// </summary>
        public HuobiSocketClientOptions(): base("wss://api.huobi.pro/ws")
        {
            SocketSubscriptionsCombineTarget = 10;
        }

        /// <summary>
        /// Copy
        /// </summary>
        /// <returns></returns>
        public HuobiSocketClientOptions Copy()
        {
            var copy = Copy<HuobiSocketClientOptions>();
            copy.BaseAddressAuthenticated = BaseAddressAuthenticated;
            return copy;
        }
    }

    /// <summary>
    /// Order book options
    /// </summary>
    public class HuobiOrderBookOptions : OrderBookOptions
    {
        /// <summary>
        /// The way the entries are merged. 0 is no merge, 2 means to combine the entries on 2 decimal places
        /// </summary>
        public int? MergeStep { get; set; }

        /// <summary>
        /// The client to use for the socket connection. When using the same client for multiple order books the connection can be shared.
        /// </summary>
        public IHuobiSocketClient? SocketClient { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mergeStep">The way the entries are merged. 0 is no merge, 2 means to combine the entries on 2 decimal places</param>
        /// <param name="socketClient">The client to use for the socket connection. When using the same client for multiple order books the connection can be shared.</param>
        public HuobiOrderBookOptions(int? mergeStep = null, IHuobiSocketClient? socketClient = null) : base("Huobi", false, false)
        {
            SocketClient = socketClient;
            MergeStep = mergeStep;
        }
    }
}
