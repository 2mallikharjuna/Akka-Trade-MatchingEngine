using System;
using System.Linq;
using System.Collections.Generic;


namespace PE.Akka.TradeEngine.Models
{
    /// <summary>
    /// OrderBook class to maintain collection of ask/bids
    /// </summary>
    public class OrderBook
    {
        private List<Order> _bidOrders;
        private List<Order> _askOrders;
        /// <summary>
        /// Get only Bid Orders
        /// </summary>
        public IReadOnlyList<Order> BidOrders
        {
            get
            {
                return _bidOrders.OrderByDescending(order => order.StockPrice).ToList();
            }
        }
        /// <summary>
        /// Get only ask orders collection
        /// </summary>
        public IReadOnlyList<Order> AskOrders
        {
            get
            {
                return _askOrders.OrderBy(order => order.StockPrice).ToList();
            }
        }
        /// <summary>
        /// Orderbook constructor
        /// </summary>
        public OrderBook()
        {
            _bidOrders = new List<Order>();
            _askOrders = new List<Order>();
        }
        /// <summary>
        /// Add the Bid order to collection
        /// </summary>
        /// <param name="order"></param>
        public void AddBidOrder(Order order)
        {
            var matchOrder = _bidOrders.Where(x => x.StockId == order.StockId).SingleOrDefault(x => order.StockPrice >= x.StockPrice);
            if (matchOrder != null)
                matchOrder.StockQuantity += order.StockQuantity;
            _bidOrders.Add(order);
        }
        /// <summary>
        /// Add ask order to collectioin
        /// </summary>
        /// <param name="order"></param>
        public void AddAskOrder(Order order)
        {
            var matchOrder = _askOrders.Where(x => x.StockId == order.StockId).SingleOrDefault(x => order.StockPrice <= x.StockPrice);
            if (matchOrder != null)
                matchOrder.StockQuantity += order.StockQuantity;
            else
                _askOrders.Add(order);
        }
        /// <summary>
        /// Remove bid order from collection
        /// </summary>
        /// <param name="orderId"></param>
        public void RemoveBidOrder(Guid orderId)
        {            
            var removedOrder = _bidOrders.FirstOrDefault(p => p.OrderId == orderId);
            _bidOrders.Remove(removedOrder);
        }
        /// <summary>
        /// Remove ask order from collection
        /// </summary>
        /// <param name="orderId"></param>
        public void RemoveAskOrder(Guid orderId)
        {            
            var removedOrder = _askOrders.FirstOrDefault(p => p.OrderId == orderId);
            _askOrders.Remove(removedOrder);            
        }
               
    }
}
