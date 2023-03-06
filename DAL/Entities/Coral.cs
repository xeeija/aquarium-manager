namespace DAL.Entities;

public class Coral : AquariumItem
{
  public CoralType CoralType { get; set; }

}

public enum CoralType
{
  HardCoral,
  SoftCoral
}
