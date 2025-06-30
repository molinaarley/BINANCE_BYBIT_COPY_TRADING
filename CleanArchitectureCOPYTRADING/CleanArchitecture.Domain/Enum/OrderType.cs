using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.Enum
{
    /// <summary>
    /// Order type
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// Limit order. An order for a set (or better) price
        /// </summary>
        Limit,
        /// <summary>
        /// Market order. An order for the best price available upon placing
        /// </summary>
        Market,
        /// <summary>
        /// Limit maker order, only available for SPOT
        /// </summary>
        LimitMaker
    }
    /*
     public enum SortFilter
{
      FirstName = 0,
      LastName = 1,
      Age = 2,
      Experience = 3
}
     */
}
