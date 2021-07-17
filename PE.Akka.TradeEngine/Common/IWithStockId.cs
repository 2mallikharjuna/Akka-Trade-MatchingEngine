using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PE.Akka.TradeEngine.Common
{
    interface IWithStockId
    {
        // <summary>
        /// The ticker symbol for a specific stock.
        /// </summary>
        string StockId { get; }
    }
}
