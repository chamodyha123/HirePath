using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace HirePathAI.API.Helpers
{
    public static class ControllerExtensions
    {
        // Pulls the acting user's Id out of the JWT "sub" claim.
        // (ASP.NET Core's JWT handler maps "sub" -> ClaimTypes.NameIdentifier
        // by default, so we check both to be safe.)
        public static int GetUserId(this ControllerBase controller)
        {
            var idClaim = controller.User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? controller.User.FindFirstValue("sub");

            if (idClaim == null || !int.TryParse(idClaim, out var userId))
                throw new UnauthorizedAccessException("User id claim missing or invalid.");

            return userId;
        }

        public static bool IsAdmin(this ControllerBase controller)
        {
            return controller.User.IsInRole("Admin");
        }
    }
}