using Akka.Actor;
using Akka.Event;
using PE.Akka.TradeEngine.Models;

namespace PE.Akka.TradeEngine.Actors
{
    /// <summary>
    /// Stock brocker receives all the successfull messages
    /// </summary>
    public class StockBroker : ReceiveActor
    {
        private readonly ILoggingAdapter log = Context.GetLogger();
        public StockBroker()
        {            
            Receive<TradeTrasaction>(tradeTrasaction => GetResponse(tradeTrasaction)); //Completed transactions
        }
        /// <summary>
        /// Response event
        /// </summary>
        /// <param name="tradeTrasaction"></param>
        private void GetResponse(TradeTrasaction tradeTrasaction)
        {            
            if (tradeTrasaction.TradeStatus == Common.TradeStatus.Listed)
                log.Info($"Trade is Listed with Price {tradeTrasaction.OrderPrice}");
            if (tradeTrasaction.TradeStatus == Common.TradeStatus.PartiallyTraded)
                log.Info($"Partially Traded {tradeTrasaction.OrderQuantity}");
            if (tradeTrasaction.TradeStatus == Common.TradeStatus.Traded)
                log.Info("Trade is successful");            

            Sender.Tell(tradeTrasaction);
        }        
    }
}
