using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NetMapManager.API.Data;
using System.Threading.Tasks;

namespace NetMapManager.API.Security
{
    public class DbAuthorizedUserRequirement : IAuthorizationRequirement { }

    public class DbAuthorizedUserHandler : AuthorizationHandler<DbAuthorizedUserRequirement>
    {
        private readonly NetMapDbContext _dbContext;

        public DbAuthorizedUserHandler(NetMapDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, DbAuthorizedUserRequirement requirement)
        {
            var identity = context.User.Identity;
            
            if (identity == null || !identity.IsAuthenticated || string.IsNullOrEmpty(identity.Name))
            {
                return; // Not authenticated
            }

            // identity.Name typically comes as "DOMAIN\Username"
            var username = identity.Name;

            var user = await _dbContext.AuthorizedUsers
                .FirstOrDefaultAsync(u => u.DomainUsername == username);

            if (user != null)
            {
                context.Succeed(requirement);
            }
            else
            {
                // Auto-provision user for development/ease of use
                var newUser = new NetMapManager.API.Models.AuthorizedUser 
                { 
                    DomainUsername = username,
                    Role = "Admin",
                    LastLogin = System.DateTime.UtcNow
                };
                
                _dbContext.AuthorizedUsers.Add(newUser);
                await _dbContext.SaveChangesAsync();

                context.Succeed(requirement);
            }
        }
    }
}
