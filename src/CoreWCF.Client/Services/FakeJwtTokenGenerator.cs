namespace CoreWCF.Client.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

public static class FakeJwtTokenGenerator
{
    private const string Secret = "this-is-a-super-secret-key-for-development-only-min-32-chars";
    
    public static string GenerateToken(HumanType humanType)
    {
        var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claimValue = GetClaimValue(humanType);

        var claims = new List<Claim>();
        
        if (claimValue != null)
        {
            claims.Add(new Claim("iscoolhuman", claimValue));
        }
        
        var token = new JwtSecurityToken(
            issuer: "fake-issuer",
            audience: "fake-audience",
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public static SymmetricSecurityKey GetSecurityKey() => 
        new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Secret));

    private static string? GetClaimValue(HumanType humanType) =>
        humanType switch
        {
            HumanType.Owner => "owner",
            HumanType.AlergicToCats => "isalergic",
            HumanType.CatLady => "catlady",
            _ => null
        };
}
