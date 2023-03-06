namespace DAL.Entities;

public abstract class AquariumItem : Entity
{
  public AquariumItem() { }

  public string Aquarium { get; set; }
  public string Name { get; set; }

  public string Species { get; set; }
  public DateTime Inserted { get; set; } = DateTime.Now;

  public int Amount { get; set; }

  public string Description { get; set; }
}
