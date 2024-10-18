using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services
{
    public class TokenService(IConfiguration config) : ITokenService
    {
        public string CreateToken(AppUser user)
        {
            var tokenkey = config["TokenKey"] ?? throw new Exception("Cannot acces tokenKey from appsettings");
            if (tokenkey.Length < 64) throw new Exception("Your tokenKey needs to be longer");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenkey));

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserName),
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = creds
            };

            var TokenHandler = new JwtSecurityTokenHandler();
            var token = TokenHandler.CreateToken(tokenDescriptor);

            return TokenHandler.WriteToken(token);
        }
    }
}
