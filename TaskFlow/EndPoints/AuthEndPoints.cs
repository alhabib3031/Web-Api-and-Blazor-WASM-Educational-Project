using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskFlow.Data;
using TaskFlow.Entity;

namespace TaskFlow.EndPoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");
        group.MapGet(
            "/login-google",
            (HttpContext context) =>
            {
                var props = new AuthenticationProperties
                {
                    RedirectUri = "/api/auth/google-response",
                };

                return Results.Challenge(props, new[] { GoogleDefaults.AuthenticationScheme });
            }
        );

        group.MapGet(
            "/google-response",
            async (HttpContext context, ApplicationDbContext db, IConfiguration config) =>
            {
                var result = await context.AuthenticateAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme
                );
                if (!result.Succeeded)
                    return Results.BadRequest("Fail to Authenticat");

                var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
                var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(email))
                    return Results.BadRequest("Google dose't that mail");

                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    user = new User { Email = email, FullName = name ?? "Google User" };

                    db.Users.Add(user);
                    await db.SaveChangesAsync();
                }

                var userClaims = new[]
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                };

                string token = GenerateMyToken(userClaims, config);

                return Results.Redirect($"http://localhost:5047/login-success?token={token}");
            }
        );
    }

    private static string GenerateMyToken(Claim[] userClaims, IConfiguration config)
    {
        var secretKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["Authentication:Jwt:Key"]!)
        );
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(userClaims),
            Expires = DateTime.UtcNow.AddDays(7), // صالحة لـ 7 أيام
            SigningCredentials = signingCredentials,
            Issuer = config["Authentication:Jwt:Issuer"],
            Audience = config["Authentication:Jwt:Audience"],
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(securityToken);
    }
}
