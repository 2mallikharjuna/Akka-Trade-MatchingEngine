using Akka.Actor;
using PE.Akka.TradeEngine.MatchEngine;
using PE.Akka.TradeEngine.Models;

namespace PE.Akka.TradeEngine.Actors
{
    /// <summary>
    /// Price change receive actor and forward event to stock brocker
    /// </summary>
    public class PriceChangeActor : ReceiveActor
    {
        private IMatcherCommand _matchingEngine;
        private readonly IActorRef _confirmationActor;
        /// <summary>
        /// PriceChangeActor class constructor
        /// </summary>
        public PriceChangeActor()
        {
            _matchingEngine = _matchingEngine ?? Matcher.Instance;
            _confirmationActor = Context.ActorOf(Props.Create(() => new StockBroker()));
            Receive<Trade>(trade => GetResponse(trade));
        }
        /// <summary>
        /// publish the successful messages to stock broker actor
        /// </summary>
        /// <param name="tradeTrasaction"></param>
        void PublishMessage(TradeTrasaction tradeTrasaction)
        {
            if (tradeTrasaction == null || tradeTrasaction.TradeStatus != Common.TradeStatus.Listed)
                return;          
            
            _confirmationActor.Ask(tradeTrasaction); //publish event for price change
        }
        /// <summary>
        /// Receive event from TradeSystemservice class 
        /// </summary>
        /// <param name="trade"></param>
        private void GetResponse(Trade trade)
        {
            TradeTrasaction tradeTrasaction = null;
            if(trade.OrderType == Common.OrderType.Buy)
                tradeTrasaction = _matchingEngine.ExecuteBuyPriceChange(trade.ToOrder());            
            else if(trade.OrderType == Common.OrderType.Sell)
                tradeTrasaction = _matchingEngine.ExecuteSellPriceChange(trade.ToOrder());
            PublishMessage(tradeTrasaction);
            Sender.Tell(tradeTrasaction, Self);
        }        
    }
}
