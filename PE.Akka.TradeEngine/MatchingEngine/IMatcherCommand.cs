using PE.Akka.TradeEngine.Models;
using System.Collections.Generic;

namespace PE.Akka.TradeEngine.MatchEngine
{
    /// <summary>
    /// IMatcherCommand interface contract
    /// </summary>
    public interface IMatcherCommand
    {
        List<TradeTrasaction> ExecuteBuy(Order buyOrder);
        List<TradeTrasaction> ExecuteSell(Order sellOrder);
        TradeTrasaction ExecuteBuyPriceChange(Order pcOrder);
        TradeTrasaction ExecuteSellPriceChange(Order pcOrder);

        OrderBook OrderBook { get; }
    }
}