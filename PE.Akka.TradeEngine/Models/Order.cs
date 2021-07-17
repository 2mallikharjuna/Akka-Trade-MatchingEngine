using PE.Akka.TradeEngine.Common;
using System;

namespace PE.Akka.TradeEngine.Models
{
    /// <summary>
    /// Order details of tricker/stock
    /// </summary>
    public class Order : IWithOrderId, IWithStockId
    {
        /// <summary>
        ///The stock ticker symbol
        /// </summary>
        public string StockId { get; private set; }
        /// <summary>
        /// The number of shares to trade
        /// </summary>
        public Decimal StockQuantity { get; set; }
        // <summary>
        /// The stock price
        /// </summary>
        public decimal StockPrice { get; set; }
        /// <summary>
        /// Order Id to initiate trade
        /// </summary>
        public Guid OrderId { get; }

        public Order(string stockId,  Guid orderId, decimal askPrice, decimal quantity)
        {
            StockId = stockId;
            OrderId = orderId;
            StockPrice = askPrice;
            StockQuantity = quantity;  
        }
    }
}
