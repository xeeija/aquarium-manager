﻿namespace Services.Auth;

public class AuthenticationInformation
{
  public String Token { get; set; }

  public long ExpirationDate { get; set; }
}
