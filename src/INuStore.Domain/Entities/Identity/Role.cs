using System;
using System.Collections.Generic;
using System.Text;

namespace INuStore.Domain.Entities.Identity
{
    public class Role : DomainEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();

    }
}
