using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace GitApp.Api.Services;

public class TokenService(JwtOptions options)
{
    public string CreateToken(UserSession session)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = options.Issuer,
            Expires = session.ExpiresAt,
            Claims = new Dictionary<string, object>
            {
                [JwtRegisteredClaimNames.Name] = session.Username,
                [JwtRegisteredClaimNames.Sid] = session.Id
            },
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }
}
