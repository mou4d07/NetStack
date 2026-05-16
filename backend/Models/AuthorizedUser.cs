using System;

namespace NetMapManager.API.Models
{
    public class AuthorizedUser
    {
        public int Id { get; set; }
        public string DomainUsername { get; set; } = string.Empty; // e.g., "DOMAIN\User"
        public string Role { get; set; } = "Viewer";
        public DateTime? LastLogin { get; set; }
    }
}
