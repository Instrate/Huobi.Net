using System;
using System.Collections.Generic;
using System.Text;

namespace Huobi.Net.Enums
{
    /// <summary>
    /// Used to 
    /// </summary>
    public enum EUsdtSwapOrderDirection
    {
        /// <summary>
        /// Long direction
        /// </summary>
        Buy,
        /// <summary>
        /// Short direction
        /// </summary>
        Sell
    }

    /// <summary>
    /// Offset direction
    /// </summary>
    public enum EUsdtSwapOrderOffset
    {
        /// <summary>
        /// Increase position
        /// </summary>
        Open,
        /// <summary>
        /// Decrease position
        /// </summary>
        Close
    }

    /// <summary>
    /// Shows the type of swap 
    /// </summary>
    public enum EUsdtSwapOrderPriceType
    {
        /// <summary>
        /// Limit Order
        /// </summary>
        Limit,
        /// <summary>
        /// BBO
        /// </summary>
        Opponent,
        /// <summary>
        /// Post-Only Order, No order limit but position limit for post-only orders
        /// </summary>
        Post_only,
        /// <summary>
        /// Optimal 5
        /// </summary>
        Optimal_5,
        /// <summary>
        /// Optimal 10
        /// </summary>
        Optimal_10,
        /// <summary>
        /// Optimal 20
        /// </summary>
        Optimal_20,
        /// <summary>
        /// IOC Order
        /// </summary>
        Ioc,
        /// <summary>
        /// FOK Order
        /// </summary>
        Fok,
        /// <summary>
        /// IOC order using the BBO price
        /// </summary>
        Opponent_ioc,
        /// <summary>
        /// optimal_5 IOC
        /// </summary>
        Optimal_5_ioc,
        /// <summary>
        /// optimal_10 IOC
        /// </summary>
        Optimal_10_ioc,
        /// <summary>
        /// optimal_20 IOC
        /// </summary>
        Optimal_20_ioc,
        /// <summary>
        /// FOK order using the BBO price
        /// </summary>
        Opponent_fok,
        /// <summary>
        /// optimal_5 FOK
        /// </summary>
        Optimal_5_fok,
        /// <summary>
        /// optimal_10 FOK
        /// </summary>
        Optimal_10_fok,
        /// <summary>
        /// optimal_20 FOK
        /// </summary>
        Optimal_20_fok
    }
}
