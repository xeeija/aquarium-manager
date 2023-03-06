namespace DAL.Entities;

public class UserAquarium : Entity
{
  public string UserID { get; set; }
  public string AquariumID { get; set; }

  public UserRole Role { get; set; }
}

public enum UserRole
{
  User,
  Admin
}
