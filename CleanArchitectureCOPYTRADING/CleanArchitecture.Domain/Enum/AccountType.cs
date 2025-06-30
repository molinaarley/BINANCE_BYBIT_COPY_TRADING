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
    public enum AccountType
    {
        /// <summary>
        /// Contract account (futures)
        /// </summary>
        [CleanArchitecture.Domain.MapAttribute.Map("CONTRACT")]
        Contract,
        /// <summary>
        /// Spot account
        /// </summary>
        [CleanArchitecture.Domain.MapAttribute.Map("SPOT")]
        Spot,
        /// <summary>
        /// Investment (defi) account
        /// </summary>
        [CleanArchitecture.Domain.MapAttribute.Map("INVESTMENT")]
        Investment,
        /// <summary>
        /// Copy trading account
        /// </summary>
        [CleanArchitecture.Domain.MapAttribute.Map("COPYTRADING")]
        CopyTrading,
        /// <summary>
        /// Option account
        /// </summary>
        [CleanArchitecture.Domain.MapAttribute.Map("OPTION")]
        Option,
        /// <summary>
        /// Funding account
        /// </summary>
        [CleanArchitecture.Domain.MapAttribute.Map("FUND")]
        Fund,
        /// <summary>
        /// Unified account
        /// </summary>
        [CleanArchitecture.Domain.MapAttribute.Map("UNIFIED")]
        Unified,
    }

}
