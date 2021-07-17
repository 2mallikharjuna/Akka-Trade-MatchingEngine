using Akka.Actor;
using PE.Akka.TradeEngine.Actors;
using PE.Akka.TradeEngine.Common;
using PE.Akka.TradeEngine.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static PE.Akka.TradeEngine.Actors.OrderBookActor;

namespace PE.Akka.TradeEngine.Services
{
    /// <summary>
    /// ITradeSystemService inteface definition
    /// </summary>
    public interface ITradeSystemService
    {
        List<TradeTrasaction> Trade(string ticker, Guid orderId, decimal price, decimal shares, OrderType orderSide);
        TradeTrasaction TradePriceUpdate(string ticker, Guid orderId, decimal newPrice, OrderType ordertype);
        IReadOnlyList<TradeTrasaction> OrderTransactionsHistory { get; }
    }
    /// <summary>
    /// TradeSystemService class implements ITradeSystemService
    /// </summary>
    public class TradeSystemService : ITradeSystemService
    {        
        //Order book actor for trade operations
        private readonly IActorRef _orderBookActor;
        //Price change actor for price change operations
        private readonly IActorRef _priceChageActor;
        /// <summary>
        /// TradeSystemService class constructor
        /// </summary>
        /// <param name="actorSystem"></param>
        public TradeSystemService(ActorSystem actorSystem)
        {
            _orderBookActor = actorSystem.ActorOf(Props.Create<OrderBookActor>(), "OrderBookActor");
            _priceChageActor = actorSystem.ActorOf(Props.Create<PriceChangeActor>(), "PriceChangeActor");
        }
        /// <summary>
        /// Create the trade order
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="orderId"></param>
        /// <param name="price"></param>
        /// <param name="shares"></param>
        /// <param name="orderSide"></param>
        /// <returns></returns>
        public List<TradeTrasaction> Trade(string ticker, Guid orderId, decimal price, decimal shares, OrderType orderSide)
        {
            //Create the Trade message
            var transMessage = new Trade(ticker, orderId, price, orderSide, shares);
            //Trade with OrderBook actor
            var task = AskFromOrderBookActor(transMessage);
            //Wait for the Task to complete. The task is completed upon the Broker receiving
            //response Trade message.
            task.Wait();
            //Return the response Trade message.
            return task.Result;
        }
        /// <summary>
        /// create the price update trade
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="orderId"></param>
        /// <param name="newPrice"></param>
        /// <param name="ordertype"></param>
        /// <returns></returns>
        public TradeTrasaction TradePriceUpdate(string ticker, Guid orderId, decimal newPrice, OrderType ordertype)
        {
            //Create the Trade message
            var transMessage = new Trade(ticker, orderId, newPrice, ordertype);
            //Trade with OrderBook actor
            var task = AskFromPriceChangeActor(transMessage);
            //Wait for the Task to complete. The task is completed upon the Broker receiving
            //response Trade message.
            task.Wait();
            //Return the response Trade message.
            return task.Result;
        }
        /// <summary>
        /// Get the list of executed transactions history
        /// </summary>
        public IReadOnlyList<TradeTrasaction> OrderTransactionsHistory
        {
            get
            {
                var task = GetTransMessages();
                task.Wait();
                return task.Result;
            }
        }
        /// <summary>
        /// Ask request for transactions history
        /// </summary>
        /// <returns></returns>
        private Task<List<TradeTrasaction>> GetTransMessages()
        {
            return _orderBookActor.Ask<List<TradeTrasaction>>(new GetMessages());
        }
        /// <summary>
        /// Ask request for Trade
        /// </summary>
        /// <param name="trade"></param>
        /// <returns></returns>
        private Task<List<TradeTrasaction>> AskFromOrderBookActor(Trade trade)
        {
            return _orderBookActor.Ask<List<TradeTrasaction>>(trade);  
        }
        /// <summary>
        /// Ask request for price change
        /// </summary>
        /// <param name="trade"></param>
        /// <returns></returns>
        private Task<TradeTrasaction> AskFromPriceChangeActor(Trade trade)
        {
            return _priceChageActor.Ask<TradeTrasaction>(trade);            
        }
    }
}
