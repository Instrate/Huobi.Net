﻿using CryptoExchange39.Net;
using CryptoExchange39.Net.Authentication;
using CryptoExchange39.Net.Converters;
using CryptoExchange39.Net.Interfaces;
using CryptoExchange39.Net.Objects;
using Huobi.Net.Converters;
using Huobi.Net.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange39.Net.ExchangeInterfaces;
using Huobi.Net.Enums;
using Huobi.Net.Interfaces;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Huobi.Net
{
    /// <summary>
    /// Client for the Huobi REST API
    /// </summary>
    public class HuobiClient : RestClient, IHuobiClient, IExchangeClient
    {
        #region fields
        private static HuobiClientOptions defaultOptions = new HuobiClientOptions();
        private static HuobiClientOptions DefaultOptions => defaultOptions.Copy();


        private const string MarketTickerEndpoint = "market/tickers";
        private const string MarketTickerMergedEndpoint = "market/detail/merged";
        private const string MarketKlineEndpoint = "market/history/kline";
        private const string MarketDepthEndpoint = "market/depth";
        private const string MarketLastTradeEndpoint = "market/trade";
        private const string MarketTradeHistoryEndpoint = "market/history/trade";
        private const string MarketDetailsEndpoint = "market/detail";
        private const string NavEndpoint = "market/etp";

        private const string MarketStatusEndpoint = "market-status";
        private const string CommonSymbolsEndpoint = "common/symbols";
        private const string CommonCurrenciesEndpoint = "common/currencys";
        private const string CommonCurrenciesAndChainsEndpoint = "reference/currencies";
        private const string ServerTimeEndpoint = "common/timestamp";

        private const string GetAccountsEndpoint = "account/accounts";
        private const string GetAssetValuationEndpoint = "account/asset-valuation";
        private const string TransferAssetValuationEndpoint = "account/transfer";
        private const string GetBalancesEndpoint = "account/accounts/{}/balance";
        private const string GetAccountHistoryEndpoint = "account/history";

        private const string GetSubAccountBalancesEndpoint = "account/accounts/{}";
        private const string TransferWithSubAccountEndpoint = "subuser/transfer";

        private const string PlaceOrderEndpoint = "order/orders/place";
        private const string OpenOrdersEndpoint = "order/openOrders";
        private const string OrdersEndpoint = "order/orders";
        private const string CancelOrderEndpoint = "order/orders/{}/submitcancel";
        private const string CancelOrderByClientOrderIdEndpoint = "order/orders/submitCancelClientOrder";
        private const string CancelOrdersByCriteriaEndpoint = "order/orders/batchCancelOpenOrders";
        private const string CancelOrdersEndpoint = "order/orders/batchcancel";
        private const string OrderInfoEndpoint = "order/orders/{}";
        private const string ClientOrderInfoEndpoint = "order/orders/getClientOrder";
        private const string OrderTradesEndpoint = "order/orders/{}/matchresults";
        private const string SymbolTradesEndpoint = "order/matchresults";
        private const string HistoryOrdersEndpoint = "order/history";

        private const string QueryDepositAddressEndpoint = "account/deposit/address";
        private const string PlaceWithdrawEndpoint = "dw/withdraw/api/create";
        private const string QueryWithdrawDepositEndpoint = "query/deposit-withdraw";
        // Swap version
        private const string UsdtSwapPlaceOrderEndpoint = "linear-swap-api/v1/swap_cross_order";
        //private const string UsdtSwapPlaceOrderInfoEndpoint = "/linear-swap-api/v1/swap_cross_order_info";
        private const string UsdtSwapMarketDataEndpoint = "linear-swap-ex/market/detail/merged?contract_code={}";
        private const string UsdtSwapOrderInfoEndpoint = "linear-swap-api/v1/swap_cross_order_info";
        private const string UsdtSwapOrderCancelEndpoint = "/linear-swap-api/v1/swap_cross_cancel";
        private const string UsdtSwapContractInfoEndpoint = "linear-swap-api/v1/swap_contract_info";
        private const string UsdtSwapClosePositionEndpoint = "linear-swap-api/v1/swap_cross_lightning_close_position";
        private const string UsdtSwapPositionInfoEndpoint = "linear-swap-api/v1/swap_cross_position_info";
        private const string UsdtSwapAccountInfoEndpoint = "linear-swap-api/v1/swap_cross_account_info";

        /// <summary>
        /// Whether public requests should be signed if ApiCredentials are provided. Needed for accurate rate limiting.
        /// </summary>
        public bool SignPublicRequests { get; }
        #endregion

        #region constructor/destructor
        /// <summary>
        /// Create a new instance of HuobiClient using the default options
        /// </summary>
        public HuobiClient() : this(DefaultOptions)
        {
        }

        /// <summary>
        /// Create a new instance of the HuobiClient with the provided options
        /// </summary>
        public HuobiClient(HuobiClientOptions options) : base("Huobi", options, options.ApiCredentials == null ? null : new HuobiAuthenticationProvider(options.ApiCredentials, options.SignPublicRequests))
        {
            SignPublicRequests = options.SignPublicRequests;
            manualParseError = true;
        }

        /// <summary>
        /// Based on keys
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="apiSecret"></param>
        public HuobiClient(string apiKey, string apiSecret) :this(new HuobiClientOptions
        {
            ApiCredentials = new ApiCredentials(apiKey, apiSecret)
        })
        {

        }
        #endregion

        #region methods
        /// <summary>
        /// Sets the default options to use for new clients
        /// </summary>
        /// <param name="options">The options to use for new clients</param>
        public static void SetDefaultOptions(HuobiClientOptions options)
        {
            defaultOptions = options;
        }

        /// <summary>
        /// Set the API key and secret
        /// </summary>
        /// <param name="apiKey">The api key</param>
        /// <param name="apiSecret">The api secret</param>
        public void SetApiCredentials(string apiKey, string apiSecret)
        {
            SetAuthenticationProvider(new HuobiAuthenticationProvider(new ApiCredentials(apiKey, apiSecret), SignPublicRequests));
        }

        /// <summary>
        /// Gets the latest ticker for all symbols
        /// </summary>
        /// <returns></returns>
        public WebCallResult<IEnumerable<HuobiMarketSignalTick>> GetTickers(CancellationToken ct = default) => GetTickersAsync(ct).Result;
        /// <summary>
        /// Gets the latest ticker for all symbols
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<HuobiMarketSignalTick>>> GetTickersAsync(CancellationToken ct = default)
        {
            //HuobiSymbolTick
            //GetUrl(MarketTickerEndpoint)
            string url = "https://api.huobi.pro/market/tickers";

            var result = await SendHuobiTimestampRequest<IEnumerable<HuobiMarketSignalTick>>(new Uri(url), HttpMethod.Get, ct).ConfigureAwait(false);
            if (!result)
                return WebCallResult<IEnumerable<HuobiMarketSignalTick>>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error!);

            return new WebCallResult<IEnumerable<HuobiMarketSignalTick>>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Item1, null);
        }

        public async Task<WebCallResult<IEnumerable<HuobiContractIndexSignalTick>>> GetTickersForContractIndexAsync(string symbol, CancellationToken ct = default)
        {
            //HuobiSymbolTick
            //GetUrl(MarketTickerEndpoint)
            string url = "https://api.hbdm.com/api/v1/contract_index";

            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol }
            };

            var result = await SendHuobiTimestampRequest<IEnumerable<HuobiContractIndexSignalTick>>(new Uri(url), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
            if (!result)
                return WebCallResult<IEnumerable<HuobiContractIndexSignalTick>>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error!);

            return new WebCallResult<IEnumerable<HuobiContractIndexSignalTick>>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Item1, null);
        }

        /// <summary>
        /// Gets the ticker, including the best bid / best ask for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the ticker for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<HuobiSymbolTickMerged> GetMergedTicker(string symbol, CancellationToken ct = default) => GetMergedTickerAsync(symbol, ct).Result;

        /// <summary>
        /// Gets the ticker, including the best bid / best ask for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the ticker for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<HuobiSymbolTickMerged>> GetMergedTickerAsync(string symbol, CancellationToken ct = default)
        {
            //symbol = symbol.ValidateHuobiSymbol();
            string url = "https://api.huobi.pro/market/detail/merged";
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol }
            };
            //checkResult: false
            var result = await SendHuobiTimestampRequest<HuobiSymbolTickMerged>(new Uri(url), HttpMethod.Get, ct, parameters).ConfigureAwait(false);

            if (!result)
                return WebCallResult<HuobiSymbolTickMerged>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error!);

            result.Data.Item1.Timestamp = result.Data.Item2;
            return new WebCallResult<HuobiSymbolTickMerged>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Item1, null);
        }

        /// <summary>
        /// Get candlestick data for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the data for</param>
        /// <param name="period">The period of a single candlestick</param>
        /// <param name="size">The amount of candlesticks</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<IEnumerable<HuobiKline>> GetKlines(string symbol, HuobiPeriod period, int size, CancellationToken ct = default) => GetKlinesAsync(symbol, period, size, ct).Result;

        /// <summary>
        /// Get candlestick data for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the data for</param>
        /// <param name="period">The period of a single candlestick</param>
        /// <param name="size">The amount of candlesticks</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<HuobiKline>>> GetKlinesAsync(string symbol, HuobiPeriod period, int size, CancellationToken ct = default)
        {
            symbol = symbol.ValidateHuobiSymbol();
            size.ValidateIntBetween(nameof(size), 0, 2000);

            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
                { "period", JsonConvert.SerializeObject(period, new PeriodConverter(false)) },
                { "size", size }
            };

            return await SendHuobiRequest<IEnumerable<HuobiKline>>(GetUrl(MarketKlineEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the order book for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to request for</param>
        /// <param name="mergeStep">The way the results will be merged together</param>
        /// <param name="limit">The depth of the book</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<HuobiOrderBook> GetOrderBook(string symbol, int mergeStep, int? limit = null, CancellationToken ct = default) => GetOrderBookAsync(symbol, mergeStep, limit, ct).Result;
        /// <summary>
        /// Gets the order book for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to request for</param>
        /// <param name="mergeStep">The way the results will be merged together</param>
        /// <param name="limit">The depth of the book</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<HuobiOrderBook>> GetOrderBookAsync(string symbol, int mergeStep, int? limit = null, CancellationToken ct = default)
        {
            symbol = symbol.ValidateHuobiSymbol();
            mergeStep.ValidateIntBetween(nameof(mergeStep), 0, 2000);
            limit?.ValidateIntValues(nameof(limit), 5, 10, 20);

            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
                { "type", "step"+mergeStep }
            };
            parameters.AddOptionalParameter("depth", limit);

            var result = await SendHuobiTimestampRequest<HuobiOrderBook>(GetUrl(MarketDepthEndpoint), HttpMethod.Get, ct, parameters, checkResult: false).ConfigureAwait(false);
            if (!result)
                return WebCallResult<HuobiOrderBook>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error!);

            result.Data.Item1.Timestamp = result.Data.Item2;
            return new WebCallResult<HuobiOrderBook>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Item1, null);
        }

        /// <summary>
        /// Gets the last trade for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to request for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<HuobiSymbolTrade> GetLastTrade(string symbol, CancellationToken ct = default) => GetLastTradeAsync(symbol, ct).Result;
        /// <summary>
        /// Gets the last trade for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to request for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<HuobiSymbolTrade>> GetLastTradeAsync(string symbol, CancellationToken ct = default)
        {
            symbol = symbol.ValidateHuobiSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol }
            };

            return await SendHuobiRequest<HuobiSymbolTrade>(GetUrl(MarketLastTradeEndpoint), HttpMethod.Get, ct, parameters, checkResult: false).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the last x trades for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get trades for</param>
        /// <param name="limit">The max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<IEnumerable<HuobiSymbolTrade>> GetTradeHistory(string symbol, int limit, CancellationToken ct = default) => GetTradeHistoryAsync(symbol, limit, ct).Result;
        /// <summary>
        /// Get the last x trades for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get trades for</param>
        /// <param name="limit">The max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<HuobiSymbolTrade>>> GetTradeHistoryAsync(string symbol, int limit, CancellationToken ct = default)
        {
            symbol = symbol.ValidateHuobiSymbol();
            limit.ValidateIntBetween(nameof(limit), 0, 2000);

            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
                { "size", limit }
            };

            return await SendHuobiRequest<IEnumerable<HuobiSymbolTrade>>(GetUrl(MarketTradeHistoryEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets 24h stats for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the data for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<HuobiSymbolDetails> GetSymbolDetails24H(string symbol, CancellationToken ct = default) => GetSymbolDetails24HAsync(symbol, ct).Result;
        /// <summary>
        /// Gets 24h stats for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the data for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<HuobiSymbolDetails>> GetSymbolDetails24HAsync(string symbol, CancellationToken ct = default)
        {
            symbol = symbol.ValidateHuobiSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol }
            };

            var result = await SendHuobiTimestampRequest<HuobiSymbolDetails>(GetUrl(MarketDetailsEndpoint), HttpMethod.Get, ct, parameters, checkResult: false).ConfigureAwait(false);
            if (!result)
                return WebCallResult<HuobiSymbolDetails>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error!);

            result.Data.Item1.Timestamp = result.Data.Item2;
            return new WebCallResult<HuobiSymbolDetails>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Item1, null);
        }

        /// <summary>
        /// Gets real time NAV for ETP
        /// </summary>
        /// <param name="symbol">The symbol to get the data for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<HuobiNav> GetNav(string symbol, CancellationToken ct = default) => GetNavAsync(symbol, ct).Result;
        /// <summary>
        /// Gets real time NAV for ETP
        /// </summary>
        /// <param name="symbol">The symbol to get the data for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<HuobiNav>> GetNavAsync(string symbol, CancellationToken ct = default)
        {
            symbol = symbol.ValidateHuobiSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol }
            };

            var result = await SendHuobiTimestampRequest<HuobiNav>(GetUrl(NavEndpoint), HttpMethod.Get, ct, parameters, checkResult: false).ConfigureAwait(false);
            if (!result)
                return WebCallResult<HuobiNav>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error!);

            return new WebCallResult<HuobiNav>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Item1, null);
        }

        /// <summary>
        /// Gets the current market status
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<HuobiMarketStatus> GetMarketStatus(CancellationToken ct = default) => GetMarketStatusAsync(ct).Result;
        /// <summary>
        /// Gets the current market status
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<HuobiMarketStatus>> GetMarketStatusAsync(CancellationToken ct = default)
        {
            return await SendHuobiV2Request<HuobiMarketStatus>(GetUrl(MarketStatusEndpoint, "2"), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a list of supported symbols
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<IEnumerable<HuobiSymbol>> GetSymbols(CancellationToken ct = default) => GetSymbolsAsync(ct).Result;
        /// <summary>
        /// Gets a list of supported symbols
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<HuobiSymbol>>> GetSymbolsAsync(CancellationToken ct = default)
        {
            return await SendHuobiRequest<IEnumerable<HuobiSymbol>>(GetUrl(CommonSymbolsEndpoint, "1"), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a list of supported currencies
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<IEnumerable<string>> GetCurrencies(CancellationToken ct = default) => GetCurrenciesAsync(ct).Result;
        /// <summary>
        /// Gets a list of supported currencies
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<string>>> GetCurrenciesAsync(CancellationToken ct = default)
        {
            return await SendHuobiRequest<IEnumerable<string>>(GetUrl(CommonCurrenciesEndpoint, "1"), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a list of supported currencies and chains
        /// </summary>
        /// <param name="currency">Filter by currency</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<IEnumerable<HuobiCurrencyInfo>> GetCurrenciesAndChains(string? currency = null, CancellationToken ct = default) => GetCurrenciesAndChainsAsync(currency, ct).Result;
        /// <summary>
        /// Gets a list of supported currencies and chains
        /// </summary>
        /// <param name="currency">Filter by currency</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<HuobiCurrencyInfo>>> GetCurrenciesAndChainsAsync(string? currency = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("currency", currency);
            return await SendHuobiV2Request<IEnumerable<HuobiCurrencyInfo>>(GetUrl(CommonCurrenciesAndChainsEndpoint, "2"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the server time
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<DateTime> GetServerTime(CancellationToken ct = default) => GetServerTimeAsync(ct).Result;
        /// <summary>
        /// Gets the server time
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<DateTime>> GetServerTimeAsync(CancellationToken ct = default)
        {
            var result = await SendHuobiRequest<string>(GetUrl(ServerTimeEndpoint, "1"), HttpMethod.Get, ct).ConfigureAwait(false);
            if (!result)
                return WebCallResult<DateTime>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error!);
            var time = (DateTime)JsonConvert.DeserializeObject(result.Data, typeof(DateTime), new TimestampConverter());
            return new WebCallResult<DateTime>(result.ResponseStatusCode, result.ResponseHeaders, time, null);
        }

        /// <summary>
        /// Gets a list of accounts associated with the apikey/secret
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<IEnumerable<HuobiAccount>> GetAccounts(CancellationToken ct = default) => GetAccountsAsync(ct).Result;
        /// <summary>
        /// Gets a list of accounts associated with the apikey/secret
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<HuobiAccount>>> GetAccountsAsync(CancellationToken ct = default)
        {
            return await SendHuobiRequest<IEnumerable<HuobiAccount>>(GetUrl(GetAccountsEndpoint, "1"), HttpMethod.Get, ct, signed: true).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a list of balances for a specific account
        /// </summary>
        /// <param name="accountId">The id of the account to get the balances for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<IEnumerable<HuobiBalance>> GetBalances(long accountId, CancellationToken ct = default) => GetBalancesAsync(accountId, ct).Result;
        /// <summary>
        /// Gets a list of balances for a specific account
        /// </summary>
        /// <param name="accountId">The id of the account to get the balances for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<HuobiBalance>>> GetBalancesAsync(long accountId, CancellationToken ct = default)
        {
            var result = await SendHuobiRequest<HuobiAccountBalances>(GetUrl(FillPathParameter(GetBalancesEndpoint, accountId.ToString(CultureInfo.InvariantCulture)), "1"), HttpMethod.Get, ct, signed: true).ConfigureAwait(false);
            if (!result)
                return WebCallResult<IEnumerable<HuobiBalance>>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error!);

            return new WebCallResult<IEnumerable<HuobiBalance>>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Data, result.Error);
        }

        /// <summary>
        /// Gets the valuation of all assets
        /// </summary>
        /// <param name="accountType">Type of account to valuate</param>
        /// <param name="valuationCurrency">The currency to get the value in</param>
        /// <param name="subUserId">The id of the sub user</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<HuobiAccountValuation> GetAssetValuation(HuobiAccountType accountType, string? valuationCurrency = null, long? subUserId = null, CancellationToken ct = default) => GetAssetValuationAsync(accountType, valuationCurrency, subUserId, ct).Result;
        /// <summary>
        /// Gets the valuation of all assets
        /// </summary>
        /// <param name="accountType">Type of account to valuate</param>
        /// <param name="valuationCurrency">The currency to get the value in</param>
        /// <param name="subUserId">The id of the sub user</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<HuobiAccountValuation>> GetAssetValuationAsync(HuobiAccountType accountType, string? valuationCurrency = null, long? subUserId = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "accountType", JsonConvert.SerializeObject(accountType, new AccountTypeConverter(false))}
            };
            parameters.AddOptionalParameter("valuationCurrency", valuationCurrency);
            parameters.AddOptionalParameter("subUid", subUserId);

            return await SendHuobiV2Request<HuobiAccountValuation>(GetUrl(GetAssetValuationEndpoint, "2"), HttpMethod.Get, ct, parameters, signed: true).ConfigureAwait(false);
        }

        /// <summary>
        /// Transfer assets between accounts
        /// </summary>
        /// <param name="fromUserId">From user id</param>
        /// <param name="fromAccountType">From account type</param>
        /// <param name="fromAccountId">From account id</param>
        /// <param name="toUserId">To user id</param>
        /// <param name="toAccountType">To account type</param>
        /// <param name="toAccountId">To account id</param>
        /// <param name="currency">Currency to transfer</param>
        /// <param name="amount">Amount to transfer</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<HuobiTransactionResult> TransferAsset(long fromUserId, HuobiAccountType fromAccountType, long fromAccountId,
            long toUserId, HuobiAccountType toAccountType, long toAccountId, string currency, decimal amount, CancellationToken ct = default)
            => TransferAssetAsync(fromUserId, fromAccountType, fromAccountId, toUserId, toAccountType, toAccountId, currency, amount, ct).Result;
        /// <summary>
        /// Transfer assets between accounts
        /// </summary>
        /// <param name="fromUserId">From user id</param>
        /// <param name="fromAccountType">From account type</param>
        /// <param name="fromAccountId">From account id</param>
        /// <param name="toUserId">To user id</param>
        /// <param name="toAccountType">To account type</param>
        /// <param name="toAccountId">To account id</param>
        /// <param name="currency">Currency to transfer</param>
        /// <param name="amount">Amount to transfer</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<HuobiTransactionResult>> TransferAssetAsync(long fromUserId, HuobiAccountType fromAccountType, long fromAccountId,
            long toUserId, HuobiAccountType toAccountType, long toAccountId, string currency, decimal amount, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "from-account-id", fromAccountId.ToString(CultureInfo.InvariantCulture)},
                { "from-user", fromUserId.ToString(CultureInfo.InvariantCulture)},
                { "from-account-type", JsonConvert.SerializeObject(fromAccountType, new AccountTypeConverter(false))},

                { "to-account-id", toAccountId.ToString(CultureInfo.InvariantCulture)},
                { "to-user", toUserId.ToString(CultureInfo.InvariantCulture)},
                { "to-account-type", JsonConvert.SerializeObject(toAccountType, new AccountTypeConverter(false))},

                { "currency", currency },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) },
            };

            return await SendHuobiRequest<HuobiTransactionResult>(GetUrl(TransferAssetValuationEndpoint, "1"), HttpMethod.Post, ct, parameters, signed: true).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a list of amount changes of specified user's account
        /// </summary>
        /// <param name="accountId">The id of the account to get the balances for</param>
        /// <param name="currency">Currency name</param>
        /// <param name="transactionTypes">Amount change types</param>
        /// <param name="startTime">Far point of time of the query window. The maximum size of the query window is 1 hour. The query window can be shifted within 30 days</param>
        /// <param name="endTime">Near point of time of the query window. The maximum size of the query window is 1 hour. The query window can be shifted within 30 days</param>
        /// <param name="sort">Sorting order (Ascending by default)</param>
        /// <param name="size">Maximum number of items in each response (from 1 to 500, default is 100)</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<IEnumerable<HuobiAccountHistory>> GetAccountHistory(long accountId, string? currency = null, IEnumerable<HuobiTransactionType>? transactionTypes = null, DateTime? startTime = null, DateTime? endTime = null, HuobiSortingType? sort = null, int? size = null, CancellationToken ct = default)
            => GetAccountHistoryAsync(accountId, currency, transactionTypes, startTime, endTime, sort, size, ct).Result;

        /// <summary>
        /// Gets a list of amount changes of specified user's account
        /// </summary>
        /// <param name="accountId">The id of the account to get the balances for</param>
        /// <param name="currency">Currency name</param>
        /// <param name="transactionTypes">Amount change types</param>
        /// <param name="startTime">Far point of time of the query window. The maximum size of the query window is 1 hour. The query window can be shifted within 30 days</param>
        /// <param name="endTime">Near point of time of the query window. The maximum size of the query window is 1 hour. The query window can be shifted within 30 days</param>
        /// <param name="sort">Sorting order (Ascending by default)</param>
        /// <param name="size">Maximum number of items in each response (from 1 to 500, default is 100)</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<HuobiAccountHistory>>> GetAccountHistoryAsync(long accountId, string? currency = null, IEnumerable<HuobiTransactionType>? transactionTypes = null, DateTime? startTime = null, DateTime? endTime = null, HuobiSortingType? sort = null, int? size = null, CancellationToken ct = default)
        {
            size?.ValidateIntBetween(nameof(size), 1, 500);

            var transactionTypeConverter = new TransactionTypeConverter(false);
            var parameters = new Dictionary<string, object>
            {
                { "account-id", accountId }
            };
            parameters.AddOptionalParameter("currency", currency);
            parameters.AddOptionalParameter("transact-types", transactionTypes == null ? null : string.Join(",", transactionTypes.Select(s => JsonConvert.SerializeObject(s, transactionTypeConverter))));
            parameters.AddOptionalParameter("start-time", ToUnixTimestamp(startTime));
            parameters.AddOptionalParameter("end-time", ToUnixTimestamp(endTime));
            parameters.AddOptionalParameter("sort", sort == null ? null : JsonConvert.SerializeObject(sort, new SortingTypeConverter(false)));
            parameters.AddOptionalParameter("size", size);

            return await SendHuobiRequest<IEnumerable<HuobiAccountHistory>>(GetUrl(GetAccountHistoryEndpoint, "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// This endpoint returns the amount changes of specified user's account.
        /// </summary>
        /// <param name="accountId">The id of the account to get the ledger for</param>
        /// <param name="currency">Currency name</param>
        /// <param name="transactionTypes">Amount change types</param>
        /// <param name="startTime">Far point of time of the query window. The maximum size of the query window is 10 days. The query window can be shifted within 30 days</param>
        /// <param name="endTime">Near point of time of the query window. The maximum size of the query window is 10 days. The query window can be shifted within 30 days</param>
        /// <param name="sort">Sorting order (Ascending by default)</param>
        /// <param name="size">Maximum number of items in each response (from 1 to 500, default is 100)</param>
        /// <param name="fromId">Only get orders with ID before or after this. Used together with the direction parameter</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<IEnumerable<HuobiLedgerEntry>> GetAccountLedger(long accountId, string? currency = null, IEnumerable<HuobiTransactionType>? transactionTypes = null, DateTime? startTime = null, DateTime? endTime = null, HuobiSortingType? sort = null, int? size = null, long? fromId = null, CancellationToken ct = default)
            => GetAccountLedgerAsync(accountId, currency, transactionTypes, startTime, endTime, sort, size, fromId, ct).Result;

        /// <summary>
        /// This endpoint returns the amount changes of specified user's account.
        /// </summary>
        /// <param name="accountId">The id of the account to get the ledger for</param>
        /// <param name="currency">Currency name</param>
        /// <param name="transactionTypes">Amount change types</param>
        /// <param name="startTime">Far point of time of the query window. The maximum size of the query window is 10 days. The query window can be shifted within 30 days</param>
        /// <param name="endTime">Near point of time of the query window. The maximum size of the query window is 10 days. The query window can be shifted within 30 days</param>
        /// <param name="sort">Sorting order (Ascending by default)</param>
        /// <param name="size">Maximum number of items in each response (from 1 to 500, default is 100)</param>
        /// <param name="fromId">Only get orders with ID before or after this. Used together with the direction parameter</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<HuobiLedgerEntry>>> GetAccountLedgerAsync(long accountId, string? currency = null, IEnumerable<HuobiTransactionType>? transactionTypes = null, DateTime? startTime = null, DateTime? endTime = null, HuobiSortingType? sort = null, int? size = null, long? fromId = null, CancellationToken ct = default)
        {
            size?.ValidateIntBetween(nameof(size), 1, 500);

            var transactionTypeConverter = new TransactionTypeConverter(false);
            var parameters = new Dictionary<string, object>
            {
                { "account-id", accountId }
            };
            parameters.AddOptionalParameter("currency", currency);
            parameters.AddOptionalParameter("transact-types", transactionTypes == null ? null : string.Join(",", transactionTypes.Select(s => JsonConvert.SerializeObject(s, transactionTypeConverter))));
            parameters.AddOptionalParameter("start-time", ToUnixTimestamp(startTime));
            parameters.AddOptionalParameter("end-time", ToUnixTimestamp(endTime));
            parameters.AddOptionalParameter("sort", sort == null ? null : JsonConvert.SerializeObject(sort, new SortingTypeConverter(false)));
            parameters.AddOptionalParameter("limit", size);
            parameters.AddOptionalParameter("fromId", fromId?.ToString(CultureInfo.InvariantCulture));

            return await SendHuobiRequest<IEnumerable<HuobiLedgerEntry>>(GetUrl(GetAccountHistoryEndpoint, "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a list of balances for a specific sub account
        /// </summary>
        /// <param name="subAccountId">The id of the sub account to get the balances for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<IEnumerable<HuobiBalance>> GetSubAccountBalances(long subAccountId, CancellationToken ct = default) => GetSubAccountBalancesAsync(subAccountId, ct).Result;
        /// <summary>
        /// Gets a list of balances for a specific sub account
        /// </summary>
        /// <param name="subAccountId">The id of the sub account to get the balances for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<HuobiBalance>>> GetSubAccountBalancesAsync(long subAccountId, CancellationToken ct = default)
        {
            var result = await SendHuobiRequest<IEnumerable<HuobiAccountBalances>>(GetUrl(FillPathParameter(GetSubAccountBalancesEndpoint, subAccountId.ToString(CultureInfo.InvariantCulture)), "1"), HttpMethod.Get, ct, signed: true).ConfigureAwait(false);
            if (!result)
                return WebCallResult<IEnumerable<HuobiBalance>>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error!);

            return new WebCallResult<IEnumerable<HuobiBalance>>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.First().Data, result.Error);
        }

        /// <summary>
        /// Transfer asset between parent and sub account
        /// </summary>
        /// <param name="subAccountId">The target sub account id to transfer to or from</param>
        /// <param name="currency">The crypto currency to transfer</param>
        /// <param name="amount">The amount of asset to transfer</param>
        /// <param name="transferType">The type of transfer</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Unique transfer id</returns>
        public WebCallResult<long> TransferWithSubAccount(long subAccountId, string currency, decimal amount, HuobiTransferType transferType, CancellationToken ct = default) =>
            TransferWithSubAccountAsync(subAccountId, currency, amount, transferType, ct).Result;
        /// <summary>
        /// Transfer asset between parent and sub account
        /// </summary>
        /// <param name="subAccountId">The target sub account id to transfer to or from</param>
        /// <param name="currency">The crypto currency to transfer</param>
        /// <param name="amount">The amount of asset to transfer</param>
        /// <param name="transferType">The type of transfer</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Unique transfer id</returns>
        public async Task<WebCallResult<long>> TransferWithSubAccountAsync(long subAccountId, string currency, decimal amount, HuobiTransferType transferType, CancellationToken ct = default)
        {
            currency.ValidateNotNull(nameof(currency));
            var parameters = new Dictionary<string, object>
            {
                { "sub-uid", subAccountId },
                { "currency", currency },
                { "amount", amount },
                { "type", JsonConvert.SerializeObject(transferType, new TransferTypeConverter(false)) }
            };

            return await SendHuobiRequest<long>(GetUrl(TransferWithSubAccountEndpoint, "1"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Places an order
        /// </summary>
        /// <param name="accountId">The account to place the order for</param>
        /// <param name="symbol">The symbol to place the order for</param>
        /// <param name="orderType">The type of the order</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price of the order. Should be omitted for market orders</param>
        /// <param name="clientOrderId">The clientOrderId the order should get</param>
        /// <param name="source">Source. defaults to SpotAPI</param>
        /// <param name="stopPrice">Stop price</param>
        /// <param name="stopOperator">Operator of the stop price</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<long> PlaceOrder(long accountId, string symbol, HuobiOrderType orderType, decimal amount, decimal? price = null, string? clientOrderId = null, SourceType? source = null, decimal? stopPrice = null, Operator? stopOperator = null, CancellationToken ct = default) =>
            PlaceOrderAsync(accountId, symbol, orderType, amount, price, clientOrderId, source, stopPrice, stopOperator, ct).Result;
        /// <summary>
        /// Places an order
        /// </summary>
        /// <param name="accountId">The account to place the order for</param>
        /// <param name="symbol">The symbol to place the order for</param>
        /// <param name="orderType">The type of the order</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price of the order. Should be omitted for market orders</param>
        /// <param name="clientOrderId">The clientOrderId the order should get</param>
        /// <param name="source">Source. defaults to SpotAPI</param>
        /// <param name="stopPrice">Stop price</param>
        /// <param name="stopOperator">Operator of the stop price</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<long>> PlaceOrderAsync(long accountId, string symbol, HuobiOrderType orderType, decimal amount, decimal? price = null, string? clientOrderId = null, SourceType? source = null, decimal? stopPrice = null, Operator? stopOperator = null, CancellationToken ct = default)
        {
            symbol = symbol.ValidateHuobiSymbol();
            if (orderType == HuobiOrderType.StopLimitBuy || orderType == HuobiOrderType.StopLimitSell)
                throw new ArgumentException("Stop limit orders not supported by API");

            var parameters = new Dictionary<string, object>
            {
                { "account-id", accountId },
                { "amount", amount },
                { "symbol", symbol },
                { "type", JsonConvert.SerializeObject(orderType, new OrderTypeConverter(false)) }
            };

            parameters.AddOptionalParameter("client-order-id", clientOrderId);
            parameters.AddOptionalParameter("source", source == null? null: JsonConvert.SerializeObject(source, new SourceTypeConverter(false)));
            parameters.AddOptionalParameter("stop-price", stopPrice);
            parameters.AddOptionalParameter("operator", stopOperator == null ? null : JsonConvert.SerializeObject(stopOperator, new OperatorConverter(false)));

            // If precision of the symbol = 1 (eg has to use whole amounts, 1,2,3 etc) Huobi doesn't except the .0 postfix (1.0) for amount
            // Issue at the Huobi side
            if (amount % 1 == 0)
                parameters["amount"] = amount.ToString(CultureInfo.InvariantCulture);

            parameters.AddOptionalParameter("price", price);
            return await SendHuobiRequest<long>(GetUrl(PlaceOrderEndpoint, "1"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a list of open orders
        /// </summary>
        /// <param name="accountId">The account id for which to get the orders for</param>
        /// <param name="symbol">The symbol for which to get the orders for</param>
        /// <param name="side">Only get buy or sell orders</param>
        /// <param name="limit">The max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<IEnumerable<HuobiOpenOrder>> GetOpenOrders(long? accountId = null, string? symbol = null, HuobiOrderSide? side = null, int? limit = null, CancellationToken ct = default) =>
            GetOpenOrdersAsync(accountId, symbol, side, limit, ct).Result;
        /// <summary>
        /// Gets a list of open orders
        /// </summary>
        /// <param name="accountId">The account id for which to get the orders for</param>
        /// <param name="symbol">The symbol for which to get the orders for</param>
        /// <param name="side">Only get buy or sell orders</param>
        /// <param name="limit">The max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<HuobiOpenOrder>>> GetOpenOrdersAsync(long? accountId = null, string? symbol = null, HuobiOrderSide? side = null, int? limit = null, CancellationToken ct = default)
        {
            symbol = symbol?.ValidateHuobiSymbol();
            if (accountId != null && symbol == null)
                throw new ArgumentException("Can't request open orders based on only the account id");

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("account-id", accountId);
            parameters.AddOptionalParameter("symbol", symbol);
            parameters.AddOptionalParameter("side", side == null ? null : JsonConvert.SerializeObject(side, new OrderSideConverter(false)));
            parameters.AddOptionalParameter("size", limit);

            return await SendHuobiRequest<IEnumerable<HuobiOpenOrder>>(GetUrl(OpenOrdersEndpoint, "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancels an open order
        /// </summary>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<long> CancelOrder(long orderId, CancellationToken ct = default) => CancelOrderAsync(orderId, ct).Result;
        /// <summary>
        /// Cancels an open order
        /// </summary>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<long>> CancelOrderAsync(long orderId, CancellationToken ct = default)
        {
            return await SendHuobiRequest<long>(GetUrl(FillPathParameter(CancelOrderEndpoint, orderId.ToString(CultureInfo.InvariantCulture)), "1"), HttpMethod.Post, ct, signed: true).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancels an open order
        /// </summary>
        /// <param name="clientOrderId">The client id of the order to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<long> CancelOrderByClientOrderId(string clientOrderId, CancellationToken ct = default) => CancelOrderByClientOrderIdAsync(clientOrderId, ct).Result;
        /// <summary>
        /// Cancels an open order
        /// </summary>
        /// <param name="clientOrderId">The client id of the order to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<long>> CancelOrderByClientOrderIdAsync(string clientOrderId, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "client-order-id", clientOrderId }
            };

            return await SendHuobiRequest<long>(GetUrl(CancelOrderByClientOrderIdEndpoint, "1"), HttpMethod.Post, ct, parameters: parameters, signed: true).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancel multiple open orders
        /// </summary>
        /// <param name="orderIds">The ids of the orders to cancel</param>
        /// <param name="clientOrderIds">The client ids of the orders to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<HuobiBatchCancelResult> CancelOrders(IEnumerable<long>? orderIds = null, IEnumerable<string>? clientOrderIds = null, CancellationToken ct = default) => CancelOrdersAsync(orderIds, clientOrderIds, ct).Result;
        /// <summary>
        /// Cancel multiple open orders
        /// </summary>
        /// <param name="orderIds">The ids of the orders to cancel</param>
        /// <param name="clientOrderIds">The client ids of the orders to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<HuobiBatchCancelResult>> CancelOrdersAsync(IEnumerable<long>? orderIds = null, IEnumerable<string>? clientOrderIds = null, CancellationToken ct = default)
        {
            if (orderIds == null && clientOrderIds == null)
                throw new ArgumentException("Either orderIds or clientOrderIds should be provided");

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("order-ids", orderIds?.Select(s => s.ToString(CultureInfo.InvariantCulture)));
            parameters.AddOptionalParameter("client-order-ids", clientOrderIds?.Select(s => s.ToString(CultureInfo.InvariantCulture)));

            return await SendHuobiRequest<HuobiBatchCancelResult>(GetUrl(CancelOrdersEndpoint, "1"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancel multiple open orders
        /// </summary>
        /// <param name="accountId">The account id used for this cancel</param>
        /// <param name="symbols">The trading symbol list (maximum 10 symbols, default value all symbols)</param>
        /// <param name="side">Filter on the direction of the trade</param>
        /// <param name="limit">The number of orders to cancel [1, 100]</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<HuobiByCriteriaCancelResult> CancelOrdersByCriteria(long? accountId = null, IEnumerable<string>? symbols = null, HuobiOrderSide? side = null, int? limit = null, CancellationToken ct = default) => CancelOrdersByCriteriaAsync(accountId, symbols, side, limit, ct).Result;
        /// <summary>
        /// Cancel multiple open orders
        /// </summary>
        /// <param name="accountId">The account id used for this cancel</param>
        /// <param name="symbols">The trading symbol list (maximum 10 symbols, default value all symbols)</param>
        /// <param name="side">Filter on the direction of the trade</param>
        /// <param name="limit">The number of orders to cancel [1, 100]</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<HuobiByCriteriaCancelResult>> CancelOrdersByCriteriaAsync(long? accountId = null, IEnumerable<string>? symbols = null, HuobiOrderSide? side = null, int? limit = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("account-id", accountId?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("symbol", symbols == null ? null : string.Join(",", symbols));
            parameters.AddOptionalParameter("side", side == null ? null : JsonConvert.SerializeObject(side, new OrderSideConverter(false)));
            parameters.AddOptionalParameter("size", limit);

            return await SendHuobiRequest<HuobiByCriteriaCancelResult>(GetUrl(CancelOrdersByCriteriaEndpoint, "1"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get details of an order
        /// </summary>
        /// <param name="orderId">The id of the order to retrieve</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<HuobiOrder> GetOrderInfo(long orderId, CancellationToken ct = default) => GetOrderInfoAsync(orderId, ct).Result;
        /// <summary>
        /// Get details of an order
        /// </summary>
        /// <param name="orderId">The id of the order to retrieve</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<HuobiOrder>> GetOrderInfoAsync(long orderId, CancellationToken ct = default)
        {
            return await SendHuobiRequest<HuobiOrder>(GetUrl(FillPathParameter(OrderInfoEndpoint, orderId.ToString(CultureInfo.InvariantCulture)), "1"), HttpMethod.Get, ct, signed: true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get details of an order by client order id
        /// </summary>
        /// <param name="clientOrderId">The client id of the order to retrieve</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<HuobiOrder> GetOrderInfoByClientOrderId(string clientOrderId, CancellationToken ct = default) => GetOrderInfoByClientOrderIdAsync(clientOrderId, ct).Result;
        /// <summary>
        /// Get details of an order by client order id
        /// </summary>
        /// <param name="clientOrderId">The client id of the order to retrieve</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<HuobiOrder>> GetOrderInfoByClientOrderIdAsync(string clientOrderId, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "clientOrderId", clientOrderId }
            };

            return await SendHuobiRequest<HuobiOrder>(GetUrl(ClientOrderInfoEndpoint, "1"), HttpMethod.Get, ct, parameters: parameters, signed: true).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a list of trades made for a specific order
        /// </summary>
        /// <param name="orderId">The id of the order to get trades for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<IEnumerable<HuobiOrderTrade>> GetOrderTrades(long orderId, CancellationToken ct = default) => GetOrderTradesAsync(orderId, ct).Result;
        /// <summary>
        /// Gets a list of trades made for a specific order
        /// </summary>
        /// <param name="orderId">The id of the order to get trades for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<HuobiOrderTrade>>> GetOrderTradesAsync(long orderId, CancellationToken ct = default)
        {
            return await SendHuobiRequest<IEnumerable<HuobiOrderTrade>>(GetUrl(FillPathParameter(OrderTradesEndpoint, orderId.ToString(CultureInfo.InvariantCulture)), "1"), HttpMethod.Get, ct, signed: true).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a list of orders
        /// </summary>
        /// <param name="symbol">The symbol to get orders for</param>
        /// <param name="states">The states of orders to return</param>
        /// <param name="types">The types of orders to return</param>
        /// <param name="startTime">Only get orders after this date</param>
        /// <param name="endTime">Only get orders before this date</param>
        /// <param name="fromId">Only get orders with ID before or after this. Used together with the direction parameter</param>
        /// <param name="direction">Direction of the results to return when using the fromId parameter</param>
        /// <param name="limit">The max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<IEnumerable<HuobiOrder>> GetOrders(IEnumerable<HuobiOrderState> states, string? symbol = null, IEnumerable<HuobiOrderType>? types = null, DateTime? startTime = null, DateTime? endTime = null, long? fromId = null, HuobiFilterDirection? direction = null, int? limit = null, CancellationToken ct = default) =>
            GetOrdersAsync(states, symbol, types, startTime, endTime, fromId, direction, limit, ct).Result;
        /// <summary>
        /// Gets a list of orders
        /// </summary>
        /// <param name="symbol">The symbol to get orders for</param>
        /// <param name="states">The states of orders to return</param>
        /// <param name="types">The types of orders to return</param>
        /// <param name="startTime">Only get orders after this date</param>
        /// <param name="endTime">Only get orders before this date</param>
        /// <param name="fromId">Only get orders with ID before or after this. Used together with the direction parameter</param>
        /// <param name="direction">Direction of the results to return when using the fromId parameter</param>
        /// <param name="limit">The max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<HuobiOrder>>> GetOrdersAsync(IEnumerable<HuobiOrderState> states, string? symbol = null, IEnumerable<HuobiOrderType>? types = null, DateTime? startTime = null, DateTime? endTime = null, long? fromId = null, HuobiFilterDirection? direction = null, int? limit = null, CancellationToken ct = default)
        {
            symbol = symbol?.ValidateHuobiSymbol();
            var stateConverter = new OrderStateConverter(false);
            var typeConverter = new OrderTypeConverter(false);
            var parameters = new Dictionary<string, object>
            {
                { "states", string.Join(",", states.Select(s => JsonConvert.SerializeObject(s, stateConverter))) }
            };
            parameters.AddOptionalParameter("symbol", symbol);
            parameters.AddOptionalParameter("start-date", startTime?.ToString("yyyy-MM-dd"));
            parameters.AddOptionalParameter("end-date", endTime?.ToString("yyyy-MM-dd"));
            parameters.AddOptionalParameter("types", types == null ? null : string.Join(",", types.Select(s => JsonConvert.SerializeObject(s, typeConverter))));
            parameters.AddOptionalParameter("from", fromId);
            parameters.AddOptionalParameter("direct", direction == null ? null : JsonConvert.SerializeObject(direction, new FilterDirectionConverter(false)));
            parameters.AddOptionalParameter("size", limit);

            return await SendHuobiRequest<IEnumerable<HuobiOrder>>(GetUrl(OrdersEndpoint, "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a list of trades for a specific symbol
        /// </summary>
        /// <param name="states">Only return trades with specific states</param>
        /// <param name="symbol">The symbol to retrieve trades for</param>
        /// <param name="types">The type of orders to return</param>
        /// <param name="startTime">Only get orders after this date</param>
        /// <param name="endTime">Only get orders before this date</param>
        /// <param name="fromId">Only get orders with ID before or after this. Used together with the direction parameter</param>
        /// <param name="direction">Direction of the results to return when using the fromId parameter</param>
        /// <param name="limit">The max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<IEnumerable<HuobiOrderTrade>> GetSymbolTrades(IEnumerable<HuobiOrderState>? states = null, string? symbol = null, IEnumerable<HuobiOrderType>? types = null, DateTime? startTime = null, DateTime? endTime = null, long? fromId = null, HuobiFilterDirection? direction = null, int? limit = null, CancellationToken ct = default) =>
            GetSymbolTradesAsync(states, symbol, types, startTime, endTime, fromId, direction, limit, ct).Result;

        /// <summary>
        /// Gets a list of trades for a specific symbol
        /// </summary>
        /// <param name="states">Only return trades with specific states</param>
        /// <param name="symbol">The symbol to retrieve trades for</param>
        /// <param name="types">The type of orders to return</param>
        /// <param name="startTime">Only get orders after this date</param>
        /// <param name="endTime">Only get orders before this date</param>
        /// <param name="fromId">Only get orders with ID before or after this. Used together with the direction parameter</param>
        /// <param name="direction">Direction of the results to return when using the fromId parameter</param>
        /// <param name="limit">The max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<HuobiOrderTrade>>> GetSymbolTradesAsync(IEnumerable<HuobiOrderState>? states = null, string? symbol = null, IEnumerable<HuobiOrderType>? types = null, DateTime? startTime = null, DateTime? endTime = null, long? fromId = null, HuobiFilterDirection? direction = null, int? limit = null, CancellationToken ct = default)
        {
            symbol = symbol?.ValidateHuobiSymbol();
            var stateConverter = new OrderStateConverter(false);
            var typeConverter = new OrderTypeConverter(false);
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("states", states == null ? null : string.Join(",", states.Select(s => JsonConvert.SerializeObject(s, stateConverter))));
            parameters.AddOptionalParameter("symbol", symbol);
            parameters.AddOptionalParameter("start-date", startTime?.ToString("yyyy-MM-dd"));
            parameters.AddOptionalParameter("end-date", endTime?.ToString("yyyy-MM-dd"));
            parameters.AddOptionalParameter("types", types == null ? null : string.Join(",", types.Select(s => JsonConvert.SerializeObject(s, typeConverter))));
            parameters.AddOptionalParameter("from", fromId);
            parameters.AddOptionalParameter("direct", direction == null ? null : JsonConvert.SerializeObject(direction, new FilterDirectionConverter(false)));
            parameters.AddOptionalParameter("size", limit);

            return await SendHuobiRequest<IEnumerable<HuobiOrderTrade>>(GetUrl(SymbolTradesEndpoint, "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a list of history orders
        /// </summary>
        /// <param name="symbol">The symbol to get orders for</param>
        /// <param name="startTime">Only get orders after this date</param>
        /// <param name="endTime">Only get orders before this date</param>
        /// <param name="direction">Direction of the results to return</param>
        /// <param name="limit">The max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<HuobiOrders> GetHistoryOrders(string? symbol = null, DateTime? startTime = null, DateTime? endTime = null, HuobiFilterDirection? direction = null, int? limit = null, CancellationToken ct = default) =>
            GetHistoryOrdersAsync(symbol, startTime, endTime, direction, limit, ct).Result;

        /// <summary>
        /// Gets a list of history orders
        /// </summary>
        /// <param name="symbol">The symbol to get orders for</param>
        /// <param name="startTime">Only get orders after this date</param>
        /// <param name="endTime">Only get orders before this date</param>
        /// <param name="direction">Direction of the results to return</param>
        /// <param name="limit">The max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<HuobiOrders>> GetHistoryOrdersAsync(string? symbol = null, DateTime? startTime = null, DateTime? endTime = null, HuobiFilterDirection? direction = null, int? limit = null, CancellationToken ct = default)
        {
            symbol = symbol?.ValidateHuobiSymbol();
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("symbol", symbol);
            parameters.AddOptionalParameter("start-time", startTime == null ? null : ToUnixTimestamp(startTime.Value).ToString());
            parameters.AddOptionalParameter("end-time", endTime == null ? null : ToUnixTimestamp(endTime.Value).ToString());
            parameters.AddOptionalParameter("direct", direction == null ? null : JsonConvert.SerializeObject(direction, new FilterDirectionConverter(false)));
            parameters.AddOptionalParameter("size", limit);

            var result = await SendHuobiTimestampRequest<IEnumerable<HuobiOrder>>(GetUrl(HistoryOrdersEndpoint, "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
            if (!result)
                return WebCallResult<HuobiOrders>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error!);

            return new WebCallResult<HuobiOrders>(result.ResponseStatusCode, result.ResponseHeaders, new HuobiOrders() { Orders = result.Data.Item1, NextTime = result.Data.Item2 }, null);
        }


        /// <summary>
        /// Parent user and sub user could query deposit address of corresponding chain, for a specific crypto currency (except IOTA).
        /// </summary>
        /// <param name="currency">Crypto currency</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<HuobiDepositAddress>>> GetDepositAddressesAsync(string currency, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>() { { "currency", currency } };
            return await SendHuobiV2Request<IEnumerable<HuobiDepositAddress>>(GetUrl(QueryDepositAddressEndpoint, "2"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }
        ///<inheritdoc cref="GetDepositAddressesAsync"/>
        public WebCallResult<IEnumerable<HuobiDepositAddress>> GetDepositAddresses(string currency, CancellationToken ct = default) => GetDepositAddressesAsync(currency, ct).Result;


        /// <summary>
        /// Parent user creates a withdraw request from spot account to an external address (exists in your withdraw address list), which doesn't require two-factor-authentication.
        /// </summary>
        /// <param name="address">The desination address of this withdraw</param>
        /// <param name="currency">Crypto currency</param>
        /// <param name="amount">The amount of currency to withdraw</param>
        /// <param name="fee">The fee to pay with this withdraw</param>
        /// <param name="chain">Set as "usdt" to withdraw USDT to OMNI, set as "trc20usdt" to withdraw USDT to TRX</param>
        /// <param name="addressTag">A tag specified for this address</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<WebCallResult<long>> PlaceWithdrawAsync(string address, string currency, decimal amount, decimal fee, string? chain = null, string? addressTag = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "address", address },
                { "currency", currency },
                { "amount", amount },
                { "fee", fee },
            };

            parameters.AddOptionalParameter("chain", chain);
            parameters.AddOptionalParameter("addr-tag", addressTag);
            return await SendHuobiRequest<long>(GetUrl(PlaceWithdrawEndpoint, "1"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }
        ///<inheritdoc cref="PlaceWithdrawAsync"/>
        public WebCallResult<long> PlaceWithdraw(string address, string currency, decimal amount, decimal fee, string? chain = null, string? addressTag = null, CancellationToken ct = default) => PlaceWithdrawAsync(address, currency, amount, fee, chain, addressTag, ct).Result;
        /// <summary>
        /// Parent user and sub user searche for all existed withdraws and deposits and return their latest status.
        /// </summary>
        /// <param name="type">Define transfer type to search</param>
        /// <param name="currency">The crypto currency to withdraw</param>
        /// <param name="from">The transfer id to begin search</param>
        /// <param name="size">The number of items to return</param>
        /// <param name="direction">the order of response</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<WithdrawDeposit>>> GetWithdrawDepositAsync(WithdrawDepositType type, string? currency = null, int? from = null, int? size = null, HuobiFilterDirection? direction = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "type", JsonConvert.SerializeObject(type, new WithdrawDepositTypeConverter(false))  },
            };

            parameters.AddOptionalParameter("currency", currency);
            parameters.AddOptionalParameter("from", from?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("size", size?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("direct", direction == null ? null : JsonConvert.SerializeObject(direction, new FilterDirectionConverter(false)));
            return await SendHuobiRequest<IEnumerable<WithdrawDeposit>>(GetUrl(QueryWithdrawDepositEndpoint, "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }
        ///<inheritdoc cref="GetWithdrawDepositAsync"/>
        public WebCallResult<IEnumerable<WithdrawDeposit>> GetWithdrawDeposit(WithdrawDepositType type, string? currency = null, int? from = null, int? size = null, HuobiFilterDirection? direction = null, CancellationToken ct = default) => GetWithdrawDepositAsync(type, currency, from, size, direction, ct).Result;

        /// <summary>
        /// Get swap order info async
        /// </summary>
        /// <param name="orderId">order id</param>
        /// <param name="contract_code">contract code</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<WebCallResult<List<UsdtSwapOrderInfo>>> UsdtSwapGetOrderInfoAsync(string orderId, string contract_code, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "order_id", orderId },
                { "contract_code", contract_code }
            };

            return await SendUsdtSwapRequest<List<UsdtSwapOrderInfo>>(GetUrl(UsdtSwapOrderInfoEndpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get swap order info sync
        /// </summary>
        /// <param name="orderId">order id</param>
        /// <param name="contract_code"> contract code</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public WebCallResult<List<UsdtSwapOrderInfo>> UsdtSwapGetOrderInfo(string orderId, string contract_code, CancellationToken ct = default)
        {
            return UsdtSwapGetOrderInfoAsync(orderId, contract_code, ct).Result;
        }

        /// <summary>
        /// Gets position info async
        /// </summary>
        /// <param name="contract_code">contract code</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<WebCallResult<List<UsdtSwapPositionInfo>>> UsdtSwapGetPositionInfoAsync(string contract_code, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "contract_code", contract_code }
            };

            

            return await SendUsdtSwapRequest<List<UsdtSwapPositionInfo>>(GetUrl(UsdtSwapPositionInfoEndpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets position info sync
        /// </summary>
        /// <param name="contract_code">contract code</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public WebCallResult<List<UsdtSwapPositionInfo>> UsdtSwapGetPositionInfo(string contract_code, CancellationToken ct = default)
        {
            return UsdtSwapGetPositionInfoAsync(contract_code, ct).Result;
        }

        /// <summary>
        /// Get Usdt-M Account info async
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<WebCallResult<List<UsdtSwapAccountInfo>>> UsdtSwapGetAccountInfoAsync(CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();

            return await SendUsdtSwapRequest<List<UsdtSwapAccountInfo>>(GetUrl(UsdtSwapAccountInfoEndpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Usdt-M Account info
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public WebCallResult<List<UsdtSwapAccountInfo>> UsdtSwapGetAccountInfo(CancellationToken ct = default)
        {
            return UsdtSwapGetAccountInfoAsync(ct).Result;
        }

        /// <summary>
        /// Close current open position async
        /// </summary>
        /// <param name="contract_code">contract code</param>
        /// <param name="direction">direction to close</param>
        /// <param name="volume">volume to close</param>
        /// <param name="channel_code">Broker channel_code</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<WebCallResult<UsdtSwapPlaceOrderDetail>> UsdtSwapClosePositionAsync(string contract_code,
            EUsdtSwapOrderDirection direction, long volume, string? channel_code = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "contract_code", contract_code },
                { "direction", direction.ToString().ToLower() },
                { "volume", volume }
            };

            if (channel_code != null)
                parameters.Add("channel_code", channel_code);

            return await SendUsdtSwapRequest<UsdtSwapPlaceOrderDetail>(GetUrl(UsdtSwapClosePositionEndpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Close current open position sync
        /// </summary>
        /// <param name="contract_code">contract_code</param>
        /// <param name="direction">direction to close</param>
        /// <param name="volume">volume to close</param>
        /// <param name="channel_code"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public WebCallResult<UsdtSwapPlaceOrderDetail> UsdtSwapClosePosition(string contract_code, 
                EUsdtSwapOrderDirection direction, long volume, string? channel_code = null, CancellationToken ct = default)
        {
            return UsdtSwapClosePositionAsync(contract_code, direction, volume, channel_code, ct).Result;
        }
        /// <summary>
        /// Place Usdt Swap Order Async
        /// </summary>
        /// <param name="contract_code"></param>
        /// <param name="direction"></param>
        /// <param name="offset"></param>
        /// <param name="price_type"></param>
        /// <param name="lever_rate"></param>
        /// <param name="volume"></param>
        /// <param name="price"></param>
        /// <param name="channel_code">Broker channel code</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<WebCallResult<UsdtSwapPlaceOrderDetail>> UsdtSwapPlaceOrderAsync(string contract_code, 
            EUsdtSwapOrderDirection direction, EUsdtSwapOrderOffset offset, EUsdtSwapOrderPriceType price_type, 
            int lever_rate, long volume, decimal? price, string? channel_code = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "contract_code", contract_code },
                { "direction", direction.ToString().ToLower() },
                { "volume", volume },
                { "offset", offset.ToString().ToLower() },
                { "lever_rate", lever_rate },
                { "order_price_type", price_type.ToString().ToLower()}
            };

            if (channel_code != null)
                parameters.Add("channel_code", channel_code);

            parameters.AddOptionalParameter("price", price?.ToString());

            return await SendUsdtSwapRequest<UsdtSwapPlaceOrderDetail>(GetUrl(UsdtSwapPlaceOrderEndpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }
        

        // !SBI/TODO://
        public async Task<WebCallResult<object>> UsdtSwapCancelOrderAsync(CancellationToken ct = default)
        {
            return await SendUsdtSwapRequest<object>(GetUrl(UsdtSwapOrderCancelEndpoint), HttpMethod.Post, ct);
        }

        /// <summary>
        /// Place Usdt Swap Order Sync
        /// </summary>
        /// <param name="contract_code"></param>
        /// <param name="direction"></param>
        /// <param name="offset"></param>
        /// <param name="price_type"></param>
        /// <param name="lever_rate"></param>
        /// <param name="volume"></param>
        /// <param name="price"></param>
        /// <param name="channel_code">Broker channel_code</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public WebCallResult<UsdtSwapPlaceOrderDetail> UsdtSwapPlaceOrder(string contract_code, 
            EUsdtSwapOrderDirection direction, EUsdtSwapOrderOffset offset, EUsdtSwapOrderPriceType price_type, 
            int lever_rate, long volume, decimal? price, string? channel_code = null, CancellationToken ct = default)
        {
            return UsdtSwapPlaceOrderAsync(contract_code, direction, offset, price_type, lever_rate, volume, price, channel_code, ct).Result;
        }

        /*
            order_id	false(more see remarks)	string	order ID（different IDs are separated by ",", maximum 10 orders can be withdrew at one time）	
            client_order_id	false(more see remarks)	string	Client order ID (different IDs are separated by ",", maximum 10 orders can be withdrew at one time)	
            contract_code	false(more see remarks)	string	contract code	swap: "BTC-USDT"... , future: "BTC-USDT-210625" ...
            pair	false(more see remarks)	string	pair	BTC-USDT
            contract_type	false(more see remarks)	string	contract type	swap, this_week, next_week, quarter, next_quarter
         */

        /// <summary>
        /// Place Usdt Swap Order Async
        /// </summary>
        /// <param name="order_id"></param>
        /// <param name="contract_code"></param>
        /// <param name="contract_type"></param>
        /// <returns></returns>
        public async Task<WebCallResult<UsdtSwapPlaceOrderDetail>> UsdtSwapCancelOrderAsync(string contract_code,
            EUsdtSwapOrderDirection direction, EUsdtSwapOrderOffset offset, EUsdtSwapOrderPriceType price_type,
            int lever_rate, long volume, decimal? price, string? channel_code = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "contract_code", contract_code },
                { "direction", direction.ToString().ToLower() },
                { "volume", volume },
                { "offset", offset.ToString().ToLower() },
                { "lever_rate", lever_rate },
                { "order_price_type", price_type.ToString().ToLower()}
            };

            if (channel_code != null)
                parameters.Add("channel_code", channel_code);

            parameters.AddOptionalParameter("price", price?.ToString());

            return await SendUsdtSwapRequest<UsdtSwapPlaceOrderDetail>(GetUrl(UsdtSwapPlaceOrderEndpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Place Usdt Swap Order Sync
        /// </summary>
        /// <param name="contract_code"></param>
        /// <param name="direction"></param>
        /// <param name="offset"></param>
        /// <param name="price_type"></param>
        /// <param name="lever_rate"></param>
        /// <param name="volume"></param>
        /// <param name="price"></param>
        /// <param name="channel_code">Broker channel_code</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public WebCallResult<UsdtSwapPlaceOrderDetail> UsdtSwapCancelOrder(string contract_code,
            EUsdtSwapOrderDirection direction, EUsdtSwapOrderOffset offset, EUsdtSwapOrderPriceType price_type,
            int lever_rate, long volume, decimal? price, string? channel_code = null, CancellationToken ct = default)
        {
            return UsdtSwapCancelOrderAsync(contract_code, direction, offset, price_type, lever_rate, volume, price, channel_code, ct).Result;
        }

        /// <summary>
        /// Get usdt swap contract info async
        /// </summary>
        /// <param name="contract_code">contract code</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<WebCallResult<List<UsdtSwapSwapInfo>>> UsdtSwapGetContractInfoAsync(string contract_code, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "contract_code", contract_code }
            };

            return await SendUsdtSwapRequest<List<UsdtSwapSwapInfo>>(GetUrl(UsdtSwapContractInfoEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Get usdt swap contract info sync
        /// </summary>
        /// <param name="contract_code">contract info</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public WebCallResult<List<UsdtSwapSwapInfo>> UsdtSwapGetContractInfo(string contract_code, CancellationToken ct = default)
        {
            return UsdtSwapGetContractInfoAsync(contract_code, ct).Result;
        }

        /// <summary>
        /// Gets UsdtSwap contract info async version
        /// </summary>
        /// <param name="contract_code"> Contract code to get info</param>
        /// <returns></returns>
        public async Task<WebCallResult<UsdtSwapMarketData>> UsdtSwapGetMarketDataAsync(string contract_code, CancellationToken ct = default)
        {
            return await SendHuobiRequest<UsdtSwapMarketData>(GetUrl(FillPathParameter(UsdtSwapMarketDataEndpoint, contract_code.ToString(CultureInfo.InvariantCulture))), HttpMethod.Get, ct, signed: false).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets UsdtSwap contract info sync version
        /// </summary>
        /// <param name="contract_code"></param>
        /// <returns></returns>
        public WebCallResult<UsdtSwapMarketData> UsdtSwapGetmarketData(string contract_code)
            => UsdtSwapGetMarketDataAsync(contract_code).Result;


        private async Task<WebCallResult<T>> SendHuobiV2Request<T>(Uri uri, HttpMethod method, CancellationToken cancellationToken, Dictionary<string, object>? parameters = null, bool signed = false, bool checkResult = true)
        {
            var result = await SendRequest<HuobiApiResponseV2<T>>(uri, method, cancellationToken, parameters, signed, checkResult).ConfigureAwait(false);
            if (!result || result.Data == null)
                return new WebCallResult<T>(result.ResponseStatusCode, result.ResponseHeaders, default, result.Error);

            if (result.Data.Code != 200)
                return new WebCallResult<T>(result.ResponseStatusCode, result.ResponseHeaders, default, new ServerError(result.Data.Code, result.Data.Message));

            return new WebCallResult<T>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Data, null);
        }

        private async Task<WebCallResult<(T, DateTime)>> SendHuobiTimestampRequest<T>(Uri uri, HttpMethod method, CancellationToken cancellationToken, Dictionary<string, object>? parameters = null, bool signed = false, bool checkResult = true)
        {
            var result = await SendRequest<HuobiBasicResponse<T>>(uri, method, cancellationToken, parameters, signed, checkResult).ConfigureAwait(false);
            if (!result || result.Data == null)
                return new WebCallResult<(T, DateTime)>(result.ResponseStatusCode, result.ResponseHeaders, default, result.Error);

            if (result.Data.ErrorCode != null)
                return new WebCallResult<(T, DateTime)>(result.ResponseStatusCode, result.ResponseHeaders, default, new ServerError($"{result.Data.ErrorCode}-{result.Data.ErrorMessage}"));

            return new WebCallResult<(T, DateTime)>(result.ResponseStatusCode, result.ResponseHeaders, (result.Data.Data, result.Data.Timestamp), null);
        }

        private async Task<WebCallResult<T>> SendHuobiRequest<T>(Uri uri, HttpMethod method, CancellationToken cancellationToken, Dictionary<string, object>? parameters = null, bool signed = false, bool checkResult = true)
        {
            var result = await SendRequest<HuobiBasicResponse<T>>(uri, method, cancellationToken, parameters, signed, checkResult).ConfigureAwait(false);
            if (!result || result.Data == null)
                return new WebCallResult<T>(result.ResponseStatusCode, result.ResponseHeaders, default, result.Error);

            if (result.Data.ErrorCode != null)
                return new WebCallResult<T>(result.ResponseStatusCode, result.ResponseHeaders, default, new ServerError(result.Data.ErrorCode, result.Data.ErrorMessage));

            return new WebCallResult<T>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Data, null);
        }

        private async Task<WebCallResult<T>> SendUsdtSwapRequest<T>(Uri uri, HttpMethod method, CancellationToken cancellationToken, Dictionary<string, object>? parameters = null, bool signed = false, bool checkResult = true)
        {
            var result = await SendRequest<HuobiResponseUsdtSwap<T>>(uri, method, cancellationToken, parameters, signed, checkResult).ConfigureAwait(false); 
            if (!result || result.Data == null)
                return new WebCallResult<T>(result.ResponseStatusCode, result.ResponseHeaders, default, result.Error);

            if (result.Data.Status != "ok")
                return new WebCallResult<T>(result.ResponseStatusCode, result.ResponseHeaders, default, new ServerError(result.Data.ErrorCode, result.Data.ErrorMessage));

            return new WebCallResult<T>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Data, null);
        }
        /// <inheritdoc />
        protected override IRequest ConstructRequest(Uri uri, HttpMethod method, Dictionary<string, object>? parameters, bool signed,
            PostParameters postParameterPosition, ArrayParametersSerialization arraySerialization, int requestId)
        {
            if (parameters == null)
                parameters = new Dictionary<string, object>();

            var uriString = uri.ToString();
            if (authProvider != null)
                parameters = authProvider.AddAuthenticationToParameters(uriString, method, parameters, signed, postParameterPosition, arraySerialization);

            if ((method == HttpMethod.Get || method == HttpMethod.Delete || postParametersPosition == PostParameters.InUri) && parameters?.Any() == true)
                uriString += "?" + parameters.CreateParamString(true, arraySerialization);

            if (method == HttpMethod.Post && signed)
            {
                var uriParamNames = new[] { "AccessKeyId", "SignatureMethod", "SignatureVersion", "Timestamp", "Signature" };
                var uriParams = parameters.Where(p => uriParamNames.Contains(p.Key)).ToDictionary(k => k.Key, k => k.Value);
                uriString += "?" + uriParams.CreateParamString(true, ArrayParametersSerialization.MultipleValues);
                parameters = parameters.Where(p => !uriParamNames.Contains(p.Key)).ToDictionary(k => k.Key, k => k.Value);
            }

            var contentType = requestBodyFormat == RequestBodyFormat.Json ? Constants.JsonContentHeader : Constants.FormContentHeader;
            var request = RequestFactory.Create(method, uriString, requestId);
            request.Accept = Constants.JsonContentHeader;

            var headers = new Dictionary<string, string>();
            if (authProvider != null)
                headers = authProvider.AddAuthenticationToHeaders(uriString, method, parameters!, signed, postParameterPosition, arraySerialization);

            foreach (var header in headers)
                request.AddHeader(header.Key, header.Value);

            if ((method == HttpMethod.Post || method == HttpMethod.Put) && postParametersPosition != PostParameters.InUri)
            {
                if (parameters?.Any() == true)
                    WriteParamBody(request, parameters, contentType);
                else
                    request.SetContent("{}", contentType);
            }

            return request;

            // proxy1
            //Proxy ALEX

            headers.Add("Accept", Constants.JsonContentHeader);
            headers.Add("Content-Type", Constants.JsonContentHeader);

            JObject proxyContent = JObject.FromObject(new
            {
                url = request.Uri.ToString(),
                request = request.Method.ToString().ToUpper(),
                debug = false,
                headers = headers,
                @params = request.Content
            });

            uri = new Uri("http://23.111.108.167/proxy/proxy_2.php");
            var request1 = RequestFactory.Create(HttpMethod.Post, uri.ToString(), requestId);
            request1.SetContent(JsonConvert.SerializeObject(proxyContent), Constants.JsonContentHeader);
            //request1.SetContent(requestBodyEmptyContent, Constants.JsonContentHeader);
            //request1.Accept = Constants.JsonContentHeader.ToString();
            return request1;
        }

        /// <inheritdoc />
        protected override Task<ServerError?> TryParseError(JToken data)
        {
            if (data["err-code"] == null && data["err-msg"] == null)
                return Task.FromResult<ServerError?>(null);

            return Task.FromResult<ServerError?>(new ServerError($"{(string)data["err-code"]!}, {(string)data["err-msg"]!}"));
        }

        /// <inheritdoc />
        protected override Error ParseErrorResponse(JToken error)
        {
            if (error["err-code"] == null || error["err-msg"] == null)
                return new ServerError(error.ToString());

            return new ServerError($"{(string)error["err-code"]!}, {(string)error["err-msg"]!}");
        }

        /// <summary>
        /// Construct url
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        protected Uri GetUrl(string endpoint, string? version = null)
        {
            return version == null ? new Uri($"{BaseAddress}{endpoint}") : new Uri($"{BaseAddress}v{version}/{endpoint}");
        }

        private static long? ToUnixTimestamp(DateTime? time)
        {
            if (time == null)
                return null;
            return (long)(time.Value - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        #endregion

        #region common interface

        public string GetSymbolName(string baseAsset, string quoteAsset) => (baseAsset + quoteAsset).ToLowerInvariant();

        async Task<WebCallResult<IEnumerable<ICommonSymbol>>> GetSymbolsAsync()
        {
            var symbols = await GetSymbolsAsync();
            return WebCallResult<IEnumerable<ICommonSymbol>>.CreateFrom(symbols);
        }

        async Task<WebCallResult<HuobiMarketSignalTick>> GetTickerAsync(string symbol)
        {
            var tickers = await GetTickersAsync();
            return new WebCallResult<HuobiMarketSignalTick>(tickers.ResponseStatusCode, tickers.ResponseHeaders,
                tickers.Data.Where(w => w.symbol == symbol).First(), tickers.Error);
        }

        async Task<WebCallResult<IEnumerable<HuobiMarketSignalTick>>> GetTickersAsync()
        {
            var tickers = await GetTickersAsync();
            return new WebCallResult<IEnumerable<HuobiMarketSignalTick>>(tickers.ResponseStatusCode, tickers.ResponseHeaders,
                tickers.Data, tickers.Error);
        }

        async Task<WebCallResult<IEnumerable<ICommonKline>>> IExchangeClient.GetKlinesAsync(string symbol, TimeSpan timespan, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            if (startTime != null || endTime != null)
                return WebCallResult<IEnumerable<ICommonKline>>.CreateErrorResult(new ArgumentError($"Huobi does not support the {nameof(startTime)}/{nameof(endTime)} parameters for the method {nameof(IExchangeClient.GetKlinesAsync)}"));

            var klines = await GetKlinesAsync(symbol, GetKlineIntervalFromTimespan(timespan), limit ?? 500);
            return WebCallResult<IEnumerable<ICommonKline>>.CreateFrom(klines);
        }

        async Task<WebCallResult<ICommonOrderBook>> IExchangeClient.GetOrderBookAsync(string symbol)
        {
            var book = await GetOrderBookAsync(symbol, 0);
            return WebCallResult<ICommonOrderBook>.CreateFrom(book);
        }

        async Task<WebCallResult<IEnumerable<ICommonRecentTrade>>> IExchangeClient.GetRecentTradesAsync(string symbol)
        {
            var trades = await GetTradeHistoryAsync(symbol, 100);
            return WebCallResult<IEnumerable<ICommonRecentTrade>>.CreateFrom(trades);
        }

        async Task<WebCallResult<ICommonOrderId>> IExchangeClient.PlaceOrderAsync(string symbol, IExchangeClient.OrderSide side, IExchangeClient.OrderType type, decimal quantity, decimal? price = null, string? accountId = null)
        {
            if (accountId == null)
                return WebCallResult<ICommonOrderId>.CreateErrorResult(new ArgumentError(
                    $"Huobi needs the {nameof(accountId)} parameter for the method {nameof(IExchangeClient.PlaceOrderAsync)}"));

            var huobiType = GetOrderType(type, side);
            var result = await PlaceOrderAsync(long.Parse(accountId), symbol, huobiType, quantity, price);
            if (!result)
                return WebCallResult<ICommonOrderId>.CreateErrorResult(result.ResponseStatusCode,
                    result.ResponseHeaders, result.Error!);
            return new WebCallResult<ICommonOrderId>(result.ResponseStatusCode, result.ResponseHeaders, new HuobiPlacedOrder()
            {
                Id = result.Data
            }, null);
        }

        async Task<WebCallResult<ICommonOrder>> IExchangeClient.GetOrderAsync(string orderId, string? symbol)
        {
            var order = await GetOrderInfoAsync(long.Parse(orderId));
            return WebCallResult<ICommonOrder>.CreateFrom(order);
        }

        async Task<WebCallResult<IEnumerable<ICommonTrade>>> IExchangeClient.GetTradesAsync(string orderId, string? symbol = null)
        {
            var result = await GetOrderTradesAsync(long.Parse(orderId));
            return WebCallResult<IEnumerable<ICommonTrade>>.CreateFrom(result);
        }

        async Task<WebCallResult<IEnumerable<ICommonOrder>>> IExchangeClient.GetOpenOrdersAsync(string? symbol)
        {
            var orders = await GetOpenOrdersAsync(symbol: symbol);
            return WebCallResult<IEnumerable<ICommonOrder>>.CreateFrom(orders);
        }

        async Task<WebCallResult<IEnumerable<ICommonOrder>>> IExchangeClient.GetClosedOrdersAsync(string? symbol)
        {
            var result = await GetOrdersAsync(
                states: new[]
                {
                    HuobiOrderState.Filled
                }, symbol);
            return WebCallResult<IEnumerable<ICommonOrder>>.CreateFrom(result);
        }

        async Task<WebCallResult<ICommonOrderId>> IExchangeClient.CancelOrderAsync(string orderId, string? symbol)
        {
            var result = await CancelOrderAsync(long.Parse(orderId));
            return new WebCallResult<ICommonOrderId>(result.ResponseStatusCode, result.ResponseHeaders,
                result ? new HuobiOrder() { Id = result.Data } : null, result.Error);
        }

        async Task<WebCallResult<IEnumerable<ICommonBalance>>> IExchangeClient.GetBalancesAsync(string? accountId = null)
        {
            if (accountId == null)
                return WebCallResult<IEnumerable<ICommonBalance>>.CreateErrorResult(new ArgumentError(
                    $"Huobi needs the {nameof(accountId)} parameter for the method {nameof(IExchangeClient.GetBalancesAsync)}"));

            var balances = await GetBalancesAsync(long.Parse(accountId));
            if (!balances)
                return WebCallResult<IEnumerable<ICommonBalance>>.CreateErrorResult(balances.ResponseStatusCode,
                    balances.ResponseHeaders, balances.Error);

            var result = new List<HuobiBalanceWrapper>();
            foreach (var balance in balances.Data)
            {
                if (balance.Type == HuobiBalanceType.Interest || balance.Type == HuobiBalanceType.Loan)
                    continue;

                var existing = result.SingleOrDefault(b => b.Asset == balance.Currency);
                if (existing == null)
                {
                    existing = new HuobiBalanceWrapper() { Asset = balance.Currency };
                    result.Add(existing);
                }

                if (balance.Type == HuobiBalanceType.Frozen)
                    existing.Frozen = balance.Balance;
                else
                    existing.Trade = balance.Balance;
            }

            return new WebCallResult<IEnumerable<ICommonBalance>>(balances.ResponseStatusCode, balances.ResponseHeaders, result, balances.Error);
        }

        private static HuobiOrderType GetOrderType(IExchangeClient.OrderType type, IExchangeClient.OrderSide side)
        {
            if (side == IExchangeClient.OrderSide.Sell)
            {
                if (type == IExchangeClient.OrderType.Limit)
                    return HuobiOrderType.LimitSell;
                return HuobiOrderType.MarketSell;
            }
            else
            {
                if (type == IExchangeClient.OrderType.Limit)
                    return HuobiOrderType.LimitBuy;
                return HuobiOrderType.MarketBuy;
            }
        }

        private static HuobiPeriod GetKlineIntervalFromTimespan(TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.FromMinutes(1)) return HuobiPeriod.OneMinute;
            if (timeSpan == TimeSpan.FromMinutes(5)) return HuobiPeriod.FiveMinutes;
            if (timeSpan == TimeSpan.FromMinutes(15)) return HuobiPeriod.FiveMinutes;
            if (timeSpan == TimeSpan.FromMinutes(30)) return HuobiPeriod.ThirtyMinutes;
            if (timeSpan == TimeSpan.FromHours(1)) return HuobiPeriod.OneHour;
            if (timeSpan == TimeSpan.FromHours(4)) return HuobiPeriod.FourHours;
            if (timeSpan == TimeSpan.FromDays(1)) return HuobiPeriod.OneDay;
            if (timeSpan == TimeSpan.FromDays(7)) return HuobiPeriod.OneWeek;
            if (timeSpan == TimeSpan.FromDays(30) || timeSpan == TimeSpan.FromDays(31)) return HuobiPeriod.OneMonth;
            if (timeSpan == TimeSpan.FromDays(365)) return HuobiPeriod.OneYear;

            throw new ArgumentException("Unsupported timespan for Huobi Klines, check supported intervals using Huobi.Net.Objects.HuobiPeriod");
        }
        #endregion
    }
}
