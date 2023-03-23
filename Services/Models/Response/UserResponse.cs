using DAL.Entities;
using Services.Auth;

namespace Services.Models.Response;

public class UserResponse
{
  public User User { get; set; }

  public AuthenticationInformation AuthInfo { get; set; }
}
