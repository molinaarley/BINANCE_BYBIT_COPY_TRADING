using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.MapAttribute
{
    public class MapAttribute : Attribute
    {
        public string[] Values { get; set; }
        public MapAttribute(params string[] maps)
        {
            Values = maps;
        }
    }
}
