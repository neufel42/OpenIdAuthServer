using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OpenIdAuthServer.Controllers.Account;

[Route("Account")]
public class AccountController : Controller
{
    // Show login page
    [HttpGet("Login")]
    public IActionResult Login(string returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    // Handle login submission
    [HttpPost("Login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string username, string password, string returnUrl)
    {
        // Validate the user's credentials (in a real app, you'd check the database)
        if (username == "user" && password == "password") // Just for demonstration
        {
            // Create a claims identity and add claims to the principal
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),  // Example claim for username
                new Claim(ClaimTypes.Role, "User")  // Example role claim
            };

            var identity = new ClaimsIdentity(claims, "password");  // "password" is the authentication type
            var principal = new ClaimsPrincipal(identity);  // Create the principal

            // Sign the user in
            await HttpContext.SignInAsync("Cookies", principal);

            // Redirect back to the return URL
            return Redirect(returnUrl ?? "/");
        }

        // If login fails, show an error
        ModelState.AddModelError("", "Invalid login attempt.");
        return View();
    }

    // Logout
    [HttpPost("Logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("Cookies");
        return Redirect("/");
    }
}
