using System.Security.Claims;
using System.Security.Principal;

namespace GamingSessionApp.Helpers
{
    public static class IdentityExtensions
    {
        public static string GetThumbnail(this IPrincipal user)
        {
            var claim = ((ClaimsIdentity)user.Identity).FindFirst("ThumbnailUrl");
            return claim?.Value;
        }
    }
}
