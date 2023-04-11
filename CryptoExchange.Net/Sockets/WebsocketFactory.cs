using System.Collections.Generic;
using CryptoExchange39.Net.Interfaces;
using CryptoExchange39.Net.Logging;

namespace CryptoExchange39.Net.Sockets
{
    /// <summary>
    /// Factory implementation
    /// </summary>
    public class WebsocketFactory : IWebsocketFactory
    {
        /// <inheritdoc />
        public IWebsocket CreateWebsocket(Log log, string url)
        {
            return new BaseSocket(log, url);
        }

        /// <inheritdoc />
        public IWebsocket CreateWebsocket(Log log, string url, IDictionary<string, string> cookies, IDictionary<string, string> headers)
        {
            return new BaseSocket(log, url, cookies, headers);
        }
    }
}
