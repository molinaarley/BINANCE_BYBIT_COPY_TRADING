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
    public enum TpslMode
    {
        /// <summary>
        /// Full  TpslMode
        /// </summary>
        [CleanArchitecture.Domain.MapAttribute.Map("Full")]
        Full,
        /// <summary>
        /// Partial TpslMode
        /// </summary>
        [CleanArchitecture.Domain.MapAttribute.Map("Partial")]
        Partial       
    }

}
