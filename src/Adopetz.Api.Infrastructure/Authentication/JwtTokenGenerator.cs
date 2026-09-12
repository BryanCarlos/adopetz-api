using Adopetz.Api.Application.Common.Interfaces;
using Adopetz.Api.Application.Common.Models;
using Adopetz.Api.Domain.Entities;
using Adopetz.Api.Infrastructure.Authentication.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Adopetz.Api.Infrastructure.Authentication
{
    internal class JwtTokenGenerator(
        IOptions<JwtSettings> jwtOptions,
        IOptions<RefreshTokenSettings> refreshTokenOptions) : ITokenGenerator
    {
        private readonly JwtSettings _jwtSettings = jwtOptions.Value;
        private readonly RefreshTokenSettings _refreshTokenSettings = refreshTokenOptions.Value;

        public TokenResult GenerateAccessToken(User user, DateTime now)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new("requires_password_change", user.RequiresPasswordChange.ToString().ToLowerInvariant())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Token));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenExpiryDate = now.AddMinutes(_jwtSettings.ExpiryMinutes);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: tokenExpiryDate,
                signingCredentials: creds
            );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return new TokenResult(token, tokenExpiryDate);
        }

        public TokenResult GenerateRefreshToken(DateTime now)
        {
            var randomNumber = new byte[_refreshTokenSettings.TokenBytes];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return new TokenResult(
                Convert.ToBase64String(randomNumber),
                now.AddDays(_refreshTokenSettings.ExpiryDays)
            );
        }
    }
}
