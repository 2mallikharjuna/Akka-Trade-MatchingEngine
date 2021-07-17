using System.Linq;
using PE.Akka.TradeEngine.Models;
using PE.Akka.TradeEngine.Common;
using System;
using Akka.Util.Internal;
using System.Collections.Generic;

namespace PE.Akka.TradeEngine.MatchEngine
{
    /// <summary>
    /// Matcher class Implements IMatcherCommand
    /// </summary>
    public sealed class Matcher : IMatcherCommand
    {
        static volatile Lazy<Matcher> _instance = new Lazy<Matcher>(() => new Matcher(), true);
        public static Matcher Instance => _instance.Value;
        /// <summary>
        /// Private constructor to allow single instance
        /// </summary>
        private Matcher() {
            OrderBook = OrderBook ?? new OrderBook();
        }
        /// <summary>
        /// OrderBook Get property
        /// </summary>
        public OrderBook OrderBook { get; }      
        /// <summary>
        /// Matcher class consrtuctor
        /// </summary>
        /// <param name="orderBook"></param>
        public Matcher(OrderBook orderBook)
        {
            OrderBook = orderBook;
        }
        /// <summary>
        /// Execute Buy command
        /// </summary>
        /// <param name="buyOrder"></param>
        /// <returns></returns>
        public List<TradeTrasaction> ExecuteBuy(Order buyOrder)
        {
            if (buyOrder == null)
                throw new ArgumentNullException(typeof(Order).ToString());
            var trades = new List<TradeTrasaction>();            
            //Give the High price buy order although ask price is less
            var matchingAskOrders = OrderBook.AskOrders.Where(p => p.StockId == buyOrder.StockId && buyOrder.StockPrice >= p.StockPrice).OrderByDescending(stock => stock.StockPrice).ToList();
            foreach (var askOrder in matchingAskOrders)
            {                
                // fill the entire order
                if (askOrder.StockQuantity >= buyOrder.StockQuantity)
                {                    
                    askOrder.StockQuantity -= buyOrder.StockQuantity;
                    if (askOrder.StockQuantity == 0)
                    {
                        trades.Add(new TradeTrasaction(buyOrder.StockId, TradeStatus.Traded, OrderType.Buy, buyOrder.StockPrice, buyOrder.StockQuantity, buyOrder.OrderId));
                        OrderBook.RemoveAskOrder(askOrder.OrderId);                        

                    }
                    else                    
                        trades.Add(new TradeTrasaction(buyOrder.StockId, TradeStatus.Traded, OrderType.Buy, buyOrder.StockPrice, buyOrder.StockQuantity, buyOrder.OrderId));
                    buyOrder.StockQuantity = 0;
                }
                // fill a partial order and continue
                else if (askOrder.StockQuantity < buyOrder.StockQuantity)
                {
                    buyOrder.StockQuantity -= askOrder.StockQuantity;
                    OrderBook.RemoveAskOrder(askOrder.OrderId);
                    trades.Add(new TradeTrasaction(buyOrder.StockId, TradeStatus.PartiallyTraded, OrderType.Buy, buyOrder.StockPrice, askOrder.StockQuantity, buyOrder.OrderId));
                }                
            }
            if (buyOrder.StockQuantity == 0)
                return trades;
            // finally add the remaining order to the list
            OrderBook.AddBidOrder(buyOrder);
            trades.Add(new TradeTrasaction(buyOrder.StockId, TradeStatus.Listed, OrderType.Buy, buyOrder.StockPrice, buyOrder.StockQuantity, buyOrder.OrderId));
            return trades;
        }
        /// <summary>
        /// Execute sell command
        /// </summary>
        /// <param name="sellOrder"></param>
        /// <returns></returns>
        public List<TradeTrasaction> ExecuteSell(Order sellOrder)
        {
            if (sellOrder == null)
                throw new ArgumentNullException(typeof(Order).ToString());
            var trades = new List<TradeTrasaction>();
            //Give the High price sell order although ask price is less
            var matchingBidOrders = OrderBook.BidOrders.Where(p => p.StockId == sellOrder.StockId && p.StockPrice >= sellOrder.StockPrice).OrderByDescending(stock => stock.StockPrice).ToList();
            foreach (var bidOrder in matchingBidOrders)
            {                
                // fill the entire order
                if (bidOrder.StockQuantity >= sellOrder.StockQuantity)
                {                    
                    bidOrder.StockQuantity -= sellOrder.StockQuantity;
                    if (bidOrder.StockQuantity == 0)
                    {
                        trades.Add(new TradeTrasaction(sellOrder.StockId, TradeStatus.Traded, OrderType.Sell, sellOrder.StockPrice, sellOrder.StockQuantity, sellOrder.OrderId));
                        OrderBook.RemoveBidOrder(bidOrder.OrderId);
                        
                    }
                    else
                        trades.Add(new TradeTrasaction(sellOrder.StockId, TradeStatus.Traded, OrderType.Sell, sellOrder.StockPrice, bidOrder.StockQuantity, sellOrder.OrderId));
                    sellOrder.StockQuantity = 0;
                }
                // fill a partial order and continue
                else if (bidOrder.StockQuantity < sellOrder.StockQuantity)
                {                    
                    sellOrder.StockQuantity -= bidOrder.StockQuantity;
                    OrderBook.RemoveBidOrder(bidOrder.OrderId);
                    trades.Add(new TradeTrasaction(sellOrder.StockId, TradeStatus.PartiallyTraded, OrderType.Sell, sellOrder.StockPrice, sellOrder.StockQuantity, sellOrder.OrderId));
                }               
            }
            if (sellOrder.StockQuantity == 0 )
                return trades;
            // finally add the remaining order to the list
            OrderBook.AddAskOrder(sellOrder);
            trades.Add(new TradeTrasaction(sellOrder.StockId, TradeStatus.Listed, OrderType.Sell, sellOrder.StockPrice, sellOrder.StockQuantity, sellOrder.OrderId));
            return trades;
        }

        /// <summary>
        /// Execute Buy price change command
        /// </summary>
        /// <param name="pcOrder"></param>
        /// <returns></returns>
        public TradeTrasaction ExecuteBuyPriceChange(Order pcOrder)
        {
            if (pcOrder == null)
                throw new ArgumentNullException(typeof(Order).ToString());
            var order = OrderBook.BidOrders.FirstOrDefault(p => p.OrderId == pcOrder.OrderId);
            if (order != null)
            {
                order.StockPrice = pcOrder.StockPrice;
                return new TradeTrasaction(order.StockId, TradeStatus.Listed, OrderType.Buy, order.StockPrice, order.StockQuantity, order.OrderId);
                
            }
            return null;
        }
        /// <summary>
        /// Execute sell price change command
        /// </summary>
        /// <param name="pcOrder"></param>
        /// <returns></returns>
        public TradeTrasaction ExecuteSellPriceChange(Order pcOrder)
        {
            if (pcOrder == null)
                throw new ArgumentNullException(typeof(Order).ToString());
            var order = OrderBook.AskOrders.FirstOrDefault(p => p.OrderId == pcOrder.OrderId);
            if (order != null)
            {
                order.StockPrice = pcOrder.StockPrice;                
                return new TradeTrasaction(order.StockId, TradeStatus.Listed, OrderType.Sell, order.StockPrice, order.StockQuantity, order.OrderId);
            }
            return null;
        }
    }
}
