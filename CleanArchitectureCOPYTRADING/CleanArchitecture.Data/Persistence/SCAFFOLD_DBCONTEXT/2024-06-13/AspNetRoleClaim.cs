using System;
using System.Collections.Generic;

namespace CleanArchitecture.Infrastructure.Persistence.SCAFFOLD_DBCONTEXT.2024-06-13
{
    public partial class AspNetRoleClaim
    {
        public int Id { get; set; }
        public string RoleId { get; set; } = null!;
        public string? ClaimType { get; set; }
        public string? ClaimValue { get; set; }

        public virtual AspNetRole Role { get; set; } = null!;
    }
}
