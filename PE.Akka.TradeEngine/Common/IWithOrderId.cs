using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PE.Akka.TradeEngine.Common
{
    interface IWithOrderId
    {
        /// <summary>
        /// Unique identifier for a specific order
        /// </summary>
        Guid OrderId { get; }
    }
}
