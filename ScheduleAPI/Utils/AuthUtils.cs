using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace ScheduleAPI.Utils;

public class AuthUtils
{
    private readonly IConfiguration _config;

    public AuthUtils(IConfiguration config)
    {
        _config = config;
    }
    
    public byte[] GetPasswordHash(string password, byte[] passwordSalt)
    {
        var passwordKey = _config.GetSection("AppSettings:PasswordKey").Value;
        var extendedPasswordSalt = passwordKey + Convert.ToBase64String(passwordSalt);

        return KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.ASCII.GetBytes(extendedPasswordSalt),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8
        );
    }
    
    public string CreateToken(int userId)
    {
        var claims = new Claim[]
        {
            new Claim("userId", userId.ToString())
        };

        var tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;
        var tokenKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                tokenKeyString != null ? tokenKeyString : ""
            )
        );

        var credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha512Signature);
            
        var descriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = credentials,
            Expires = DateTime.Now.AddDays(1)
        };

        var handler = new JwtSecurityTokenHandler();
            
        var token = handler.CreateToken(descriptor);

        return handler.WriteToken(token);
    }
}