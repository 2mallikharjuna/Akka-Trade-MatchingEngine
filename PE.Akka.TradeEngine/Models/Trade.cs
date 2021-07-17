using System;
using PE.Akka.TradeEngine.Common;

namespace PE.Akka.TradeEngine.Models
{
    /// <summary>
    /// Ask/Sell trade transaction
    /// </summary>
    public class Trade : Order
    {           
        /// <summary>
        /// The type of trade, TradeType.Buy or TradeType.Sell
        /// </summary>
        public OrderType OrderType { get; private set; }
                
        /// <summary>
        /// The time the TrademMessage was created,
        /// </summary>
        public DateTime CreateTime { get; private set; }
        /// <summary>
        /// An arbitrary string that provides additional or descriptive
        /// information about the message - currently not using
        /// </summary>

        /// <summary>
        /// Get/Set Messsage
        /// </summary>
        public string Message { get; private set; }
        /// <summary>
        /// Trade class constructor
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="askPrice"></param>
        /// <param name="shares"></param>
        /// <param name="tradeOrderType"></param>
        /// <param name="orderId"></param>
        /// <param name="message"></param>

        public Trade(string ticker, Guid orderId, decimal askPrice, OrderType tradeOrderType, decimal shares, string message = null)
            : base(ticker, orderId, askPrice, shares)
        {
            OrderType = tradeOrderType;
            CreateTime = DateTime.UtcNow;
            Message = message;
        }

        /// <summary>
        ///  Trade class constructor
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="askPrice"></param>
        /// <param name="shares"></param>
        /// <param name="tradeOrderType"></param>
        /// <param name="orderId"></param>
        /// <param name="message"></param>
        public Trade(string ticker, Guid orderId, decimal newPrice, OrderType tradeOrderType, string message = null)
            : base(ticker, orderId, newPrice, 0)
        {
            OrderType = tradeOrderType;           
            CreateTime = DateTime.UtcNow;            
            Message = message;
        }
        /// <summary>
        /// Create order type object
        /// </summary>
        /// <returns></returns>
        internal Order ToOrder()
        {
            return new Order(StockId, OrderId, StockPrice, StockQuantity);
        }

    }
}
