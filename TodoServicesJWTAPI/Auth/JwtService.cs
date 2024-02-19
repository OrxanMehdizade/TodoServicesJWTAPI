using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TodoServicesJWTAPI.Auth
{
    public class JwtService : IJwtService
    {
        private readonly JwtConfig _jwtConfig;

        public JwtService(JwtConfig jwtConfig)
        {
            _jwtConfig = jwtConfig;
        }

        public string GenerateSecurityToken(string id, string email, IEnumerable<string> roles, IEnumerable<Claim> userClaims)
        {
            var claims = new[]
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType,email),
                new Claim("userId",id),
                new Claim(ClaimsIdentity.DefaultRoleClaimType,string.Join(",",roles)),

            }.Concat(userClaims);
            //var expires = DateTime.Now.AddMinutes(_jwtConfig.ExpiresMinutes);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
            var signingCredentials= new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                expires: DateTime.Now.AddMinutes(_jwtConfig.ExpiresMinutes + 300000),
                signingCredentials: signingCredentials,
                claims:claims
                );
            var accessToken= new JwtSecurityTokenHandler().WriteToken(token);

            return accessToken;
        }
    }
}
