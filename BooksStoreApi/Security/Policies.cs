using Microsoft.AspNetCore.Authorization;

namespace BooksStore.Api.Security
{
    public class Policies
    {
        public const string User = "User";
        public static AuthorizationPolicy UserPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        }
    }
}

