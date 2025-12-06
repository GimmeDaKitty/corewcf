using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace CoreWCF.Client.Services;

public class FakeJwtTokenProvider
{
    private HumanType _humanType;
    private const string Secret = "this-is-a-super-secret-key-for-development-only-min-32-chars";

    public void SetScope(HumanType humanType)
    {
        _humanType = humanType;
    }
    
    public string GenerateToken()
    {
        var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claimValue = GetClaimValue(_humanType);

        var claims = new List<Claim>();
        claims.Add(new Claim("iscoolhuman", claimValue));
        
        var token = new JwtSecurityToken(
            issuer: "https://fake-issuer.com",
            audience: "fake-audience",
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private static SymmetricSecurityKey GetSecurityKey() => 
        new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Secret));

    private static string GetClaimValue(HumanType humanType) =>
        humanType switch
        {
            HumanType.Owner => "owner",
            HumanType.AlergicToCats => "isalergic",
            HumanType.CatLady => "catlady",
            _ => "notacoolhuman"
        };
}
