using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Core.Helpers;

public static class JwtHelper
{
    public static string GenerateToken(User user, JwtSettings settings, int durationInMinutes, bool includeDetailedClaims = true)
    {
        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        };
        
        if (includeDetailedClaims)
        {
            claims = claims.Concat(new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            }).ToArray();
        }
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecurityKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(durationInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string GetUserIdFromToken(string token, JwtSettings settings)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenValidationParams = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecurityKey)),
            ValidAudience = settings.Audience,
            ValidIssuer = settings.Issuer,
        };

        tokenHandler.ValidateToken(token, tokenValidationParams, out SecurityToken validatedToken);
        var jwtToken = (JwtSecurityToken)validatedToken;
        return jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
    }
}