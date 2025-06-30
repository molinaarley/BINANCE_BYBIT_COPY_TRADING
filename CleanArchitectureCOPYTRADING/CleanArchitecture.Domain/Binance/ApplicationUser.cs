using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Domain.Binance
{
    public class ApplicationUser : IdentityUser
    {
        public string LastName { get; set; }

        public string Name { get; set; }
    }
}
