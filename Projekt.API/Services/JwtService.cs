using Microsoft.IdentityModel.Tokens;
using Projekt.API.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Projekt.API.Services
{
    //Serwis definiujący dla Jwt
    public interface IJwtService
    {
        string GenerateToken(User user);
        ClaimsPrincipal? ValidateToken(string token);
    }
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _tokenExpirationMinutes;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
            var jwtSettings = _configuration.GetSection("JwtSettings");
            _secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey nie jest skonfigurowany");
            _issuer = jwtSettings["Issuer"] ?? "ProjektAPI";
            _audience = jwtSettings["Audience"] ?? "ProjektClient";
            _tokenExpirationMinutes = int.Parse(jwtSettings["TokenExpirationInMinutes"] ?? "60");
        }

        public string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("userId", user.ID.ToString())
            };

            // Jeśli użytkownik jest klientem to dostaje też ClientId
            if (user.Role == UserRole.Client && user.Client != null)
            {
                claims.Add(new Claim("clientId", user.Client.ID.ToString()));
            }

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_tokenExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _issuer,
                    ValidAudience = _audience,
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch
            {
                return null;
            }
        }

    }
}
