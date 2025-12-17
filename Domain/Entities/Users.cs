using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool TwoFactorEnable { get; set; } = false;
        public string? TwoFactorSecret { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;    
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }




}
