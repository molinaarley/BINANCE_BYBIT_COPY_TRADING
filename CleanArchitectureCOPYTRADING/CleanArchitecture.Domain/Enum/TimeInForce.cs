using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Domain.MapAttribute;

namespace CleanArchitecture.Domain.Enum
{
    /// <summary>
    /// Time in force
    /// </summary>
    public enum TimeInForce
    {
        /// <summary>
        /// Good till canceled by user
        /// </summary>
        [CleanArchitecture.Domain.MapAttribute.Map("GTC")]
        GoodTillCanceled,
        /// <summary>
        /// Fill at least partially upon placing or cancel
        /// </summary>
        [CleanArchitecture.Domain.MapAttribute.Map("IOC")]
        ImmediateOrCancel,
        /// <summary>
        /// Fill fully upon placing or cancel
        /// </summary>
        [CleanArchitecture.Domain.MapAttribute.Map("FOK")]
        FillOrKill,
        /// <summary>
        /// Only place order if the order is added to the order book instead of being filled immediately
        /// </summary>
        PostOnly
    }
}
