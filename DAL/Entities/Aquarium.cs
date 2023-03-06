namespace DAL.Entities;

public class Aquarium : Entity
{
  public string Name { get; set; }

  public double Depth { get; set; }
  public double Length { get; set; }
  public double Height { get; set; }

  public WaterType WaterType { get; set; }

}
