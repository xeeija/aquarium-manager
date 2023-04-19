using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DAL;
using DAL.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Services.Auth;

public class Authentication
{
  UnitOfWork unit = null;

  public Authentication(UnitOfWork unit)
  {
    this.unit = unit;
  }

  const string secret = "8hHk=ad9d/hD3j9dAW983)7dhKhdBh8&2a1!)jD/aA^d";
  const string issuer = "http://aquariumissuer.com";
  const string audience = "http://aquariumaudience.com";
  SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));

  public async Task<AuthenticationInformation> Authenticate(User user)
  {
    if (user != null)
    {
      AuthenticationInformation info = new AuthenticationInformation();

      DateTime expires = DateTime.UtcNow.AddDays(7);

      info.ExpirationDate = new DateTimeOffset(expires).ToUnixTimeSeconds();

      var tokenHandler = new JwtSecurityTokenHandler();
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new Claim[]
        {
          new Claim(ClaimTypes.NameIdentifier, user.ID),
          new Claim(ClaimTypes.GivenName, user.FullName),
          new Claim(ClaimTypes.Email, user.Email),
        }),
        Expires = expires,
        Issuer = issuer,
        Audience = audience,
        SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
      };

      var token = tokenHandler.CreateToken(tokenDescriptor);
      info.Token = tokenHandler.WriteToken(token);

      return info;
    }

    return null;
  }



  public async Task<bool> ValidateCurrentToken(string token)
  {
    var tokenHandler = new JwtSecurityTokenHandler();
    try
    {
      var val = tokenHandler.ValidateToken(token, ValidationParams, out SecurityToken validatedToken);
    }
    catch
    {
      return false;
    }
    return true;
  }

  public static TokenValidationParameters ValidationParams
  {
    get
    {
      return new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret))
      };
    }
  }

  public string GetEmailByToken(string token)
  {

    var tokenHandler = new JwtSecurityTokenHandler();
    try
    {
      string finaltoken = token;

      if (token.StartsWith("Bearer "))
      {
        finaltoken = token.Replace("Bearer ", "");
      }

      var val = tokenHandler.ValidateToken(finaltoken, ValidationParams, out SecurityToken validatedToken);

      if (val.HasClaim(x => x.Type == ClaimTypes.Email))
      {
        Claim claim = val.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

        if (claim != null && !string.IsNullOrEmpty(claim.Value))
        {
          return claim.Value;
        }
      }
    }
    catch
    {
      return null;
    }
    return null;
  }

  public string GetClaim(string token, string claimType)
  {
    var tokenHandler = new JwtSecurityTokenHandler();
    var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

    var stringClaimValue = securityToken.Claims.First(claim => claim.Type == claimType).Value;
    return stringClaimValue;
  }
}
