using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Services.Interfaces;
using LibraryManagement.Api.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryManagement.Api.Services
{
    public partial class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSetting _jwt;
        private readonly UserManager<IdentityUser> _userManager;

        public JwtTokenService(IOptions<JwtSetting> options, UserManager<IdentityUser> userManager)
        {
            _jwt = options.Value;
            _userManager = userManager;
        }

        public async Task<JwtResultDto> GenerateToken(IdentityUser identity)
        {
            var expired = DateTime.UtcNow.AddMinutes(_jwt.expires);

            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier , identity.Id) ,
                new (ClaimTypes.Name , identity.UserName!)
            };

            var userRoles = await _userManager.GetRolesAsync(identity);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expired,
                Issuer = _jwt.validIssuer,
                Audience = _jwt.ValidAudiences,
                SigningCredentials = new SigningCredentials
                (new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key)), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(descriptor);
            return new JwtResultDto
            {
                Token = tokenHandler.WriteToken(securityToken),
                ExpiresAt = expired,
            };
        }
    }
}
