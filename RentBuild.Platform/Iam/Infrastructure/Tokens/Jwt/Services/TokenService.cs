using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using RentBuild.Platform.Iam.Application.Internal.OutboundServices;
using RentBuild.Platform.Iam.Domain.Model.Aggregates;
namespace RentBuild.Platform.Iam.Infrastructure.Tokens.Jwt.Services;

public sealed record JwtSettings(string Key, string Issuer, string Audience);
public sealed class TokenService(JwtSettings settings) : ITokenService
{
    public string Issue(User user)
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim(ClaimTypes.Role, user.Role), new Claim(ClaimTypes.Email, user.Email), new Claim("tokenVersion", user.TokenVersion.ToString()), new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) };
        var token = new JwtSecurityToken(settings.Issuer, settings.Audience, claims, expires: DateTime.UtcNow.AddHours(2), signingCredentials: new SigningCredentials(new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(settings.Key)), SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}