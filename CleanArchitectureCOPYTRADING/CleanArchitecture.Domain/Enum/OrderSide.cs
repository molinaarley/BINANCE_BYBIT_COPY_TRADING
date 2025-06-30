using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Domain.MapAttribute;

namespace CleanArchitecture.Domain.Enum
{
    /// <summary>
    /// Side of an order
    /// </summary>
    public enum OrderSide
    {
        /// <summary>
        /// Buy
        /// </summary>
        [CleanArchitecture.Domain.MapAttribute.Map("Buy")]
        Buy,
        /// <summary>
        /// Sell
        /// </summary>
        [CleanArchitecture.Domain.MapAttribute.Map("Sell")]
        Sell,
        /// <summary>
        /// Long
        /// </summary>
        [CleanArchitecture.Domain.MapAttribute.Map("Long")]
        Long,
        /// <summary>
        /// Court
        /// </summary>
        [CleanArchitecture.Domain.MapAttribute.Map("Court")]
        Court,


        /// <summary>
        /// None
        /// </summary>
        [CleanArchitecture.Domain.MapAttribute.Map("None")]
        None
    }
}
