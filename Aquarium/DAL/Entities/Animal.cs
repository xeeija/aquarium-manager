namespace DAL.Entities;

public class Animal : AquariumItem
{
  public DateTime DeathDate { get; set; }

  public bool IsAlive { get; set; }
}
