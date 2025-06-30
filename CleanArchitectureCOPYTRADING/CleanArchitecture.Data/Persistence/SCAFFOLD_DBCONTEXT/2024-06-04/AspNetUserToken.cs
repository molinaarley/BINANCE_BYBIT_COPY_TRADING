using System;
using System.Collections.Generic;

namespace CleanArchitecture.Infrastructure.Persistence.SCAFFOLD_DBCONTEXT.2024-06-04
{
    public partial class AspNetUserToken
    {
        public string UserId { get; set; } = null!;
        public string LoginProvider { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Value { get; set; }

        public virtual AspNetUser User { get; set; } = null!;
    }
}
