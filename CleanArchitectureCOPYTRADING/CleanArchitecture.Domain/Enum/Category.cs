using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CleanArchitecture.Domain.MapAttribute;

namespace CleanArchitecture.Domain.Enum
{
    /// <summary>
    /// Account type
    /// </summary>
    [JsonConverter(typeof(EnumConverter))]
    public enum Category
    {
        /// <summary>
        /// Linear perpetual, including USDC perp.
        /// </summary>
        [CleanArchitecture.Domain.MapAttribute.Map("linear")]
        Linear,
        /// <summary>
        /// Inverse futures, including inverse perpetual, inverse futures.
        /// </summary>
        [CleanArchitecture.Domain.MapAttribute.Map("inverse")]
        Inverse,
        /// <summary>
        /// Spot
        /// </summary>
        [CleanArchitecture.Domain.MapAttribute.Map("spot")]
        Spot,
        /// <summary>
        /// USDC Option
        /// </summary>
        [CleanArchitecture.Domain.MapAttribute.Map("option")]
        Option,
        /// <summary>
        /// Category not passed
        /// </summary>
        [CleanArchitecture.Domain.MapAttribute.Map("")]
        Undefined
    }

}
