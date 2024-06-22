using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Text;
using WebAPI.Dtos;
using WebAPI.Models;

namespace WebAPI.Utilities
{
    public class Token
    {
        private readonly string _secret;
        public Token(string secret)
        {
            _secret = secret;
        }

        public async Task<UserResponseDto> Generate(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = await Task.Run(() =>
            {
                var key = Encoding.ASCII.GetBytes(_secret);
                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(
                    [
                        new Claim(ClaimTypes.Name, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.Role)
                    ]),
                    Expires = DateTime.UtcNow.AddDays(14),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                return tokenHandler.CreateToken(tokenDescriptor);

            });

            UserResponseDto userResponseDto = new UserResponseDto()
            {
                Username = user.Username,
                Token = tokenHandler.WriteToken(token)
            };

            return userResponseDto;
        }
    }
}
