using Akka.Actor;
using Akka.Persistence;
using PE.Akka.TradeEngine.MatchEngine;
using PE.Akka.TradeEngine.Models;
using System.Collections.Generic;
using System.Linq;

namespace PE.Akka.TradeEngine.Actors
{
    /// <summary>
    /// OrderBook Actor class to interact with Matcher 
    /// </summary>
    public class OrderBookActor : ReceivePersistentActor
    {
        public class GetMessages { }
        private List<TradeTrasaction> _msgs = new List<TradeTrasaction>(); //INTERNAL STATE
        private IMatcherCommand _matchingEngine;
        private readonly IActorRef _confirmationActor;
        public override string PersistenceId => "OrderBook-Id";
        /// <summary>
        /// OrderBook class constructor
        /// </summary>
        public OrderBookActor()
        {
            _matchingEngine = _matchingEngine ?? Matcher.Instance;
            _confirmationActor = Context.ActorOf(Props.Create(() => new StockBroker()));
            // recover            
            Recovers();
            // commad
            Commands();
        }
        /// <summary>
        /// OrderBook constructor with Dependency injection
        /// </summary>
        /// <param name="matchingEngine"></param>
        public OrderBookActor(IMatcherCommand matchingEngine) : this()
        {
            _matchingEngine = matchingEngine;
            _confirmationActor = Context.ActorOf(Props.Create(() => new StockBroker()));
            Commands();
        }
        /// <summary>
        /// Recovers
        /// </summary>
        private void Recovers()
        {
            Recover<Trade>(trade => {
                if (trade.OrderType == Common.OrderType.Sell)
                {
                    var tradeTrans = Ask(trade);
                    PublishMessage(tradeTrans);
                }
            });
            Recover<Trade>(trade => {
                if (trade.OrderType == Common.OrderType.Buy)
                {
                    var tradeTrans = Bid(trade);
                    PublishMessage(tradeTrans);
                }
                
            });
            Recover<GetMessages> (get => Sender.Tell(_msgs));
        }

        /// <summary>
        /// Commands
        /// </summary>
        private void Commands()
        {
            Command<Trade>(trade =>
            {
                if (trade.OrderType == Common.OrderType.Sell)
                {
                    //Execute sell
                    var tradeTrasactions = Ask(trade);
                    PublishMessage(tradeTrasactions);
                    Sender.Tell(tradeTrasactions);
                }
                else if (trade.OrderType == Common.OrderType.Buy)
                {
                    //Execute buy
                    var tradeTrasactions = Bid(trade);
                    PublishMessage(tradeTrasactions);
                    Sender.Tell(tradeTrasactions);
                }

            });
            Command<GetMessages>(get => {
                Sender.Tell(_msgs.OrderBy(x => x.CreateTime).ToList());
            });
        }

        /// <summary>
        /// Ask function
        /// </summary>
        /// <param name="sell"></param>
        /// <returns></returns>
        List<TradeTrasaction> Ask(Trade sell)
        {            
            return _matchingEngine.ExecuteSell(sell.ToOrder());
        }
        /// <summary>
        /// Bid function
        /// </summary>
        /// <param name="buy"></param>
        /// <returns></returns>
        List<TradeTrasaction> Bid(Trade buy)
        {            
            return _matchingEngine.ExecuteBuy(buy.ToOrder());
        }
        /// <summary>
        /// Publish the message to stock brocker
        /// </summary>
        /// <param name="tradeTrasactions"></param>
        void PublishMessage(List<TradeTrasaction> tradeTrasactions)
        {
            tradeTrasactions.ForEach(trans =>
            {
                if (trans.TradeStatus == Common.TradeStatus.PartiallyTraded || trans.TradeStatus == Common.TradeStatus.Traded)
                    _confirmationActor.Ask(trans);
                _msgs.Add(trans);
            });
        }
        
    }
}
