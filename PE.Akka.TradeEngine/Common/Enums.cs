using System;

namespace PE.Akka.TradeEngine.Common
{
    /// <summary>
    /// enumerator list for Order types
    /// </summary>
    public enum OrderType
    {
        Buy,
        Sell          
    }
    
    /// <summary>
    /// Trade status
    /// </summary>
    public enum TradeStatus
    {
        None,
        Listed,
        Traded,
        PartiallyTraded,
        NotTraded,
        Invalid
    }
}
