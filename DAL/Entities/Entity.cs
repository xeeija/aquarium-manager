namespace DAL.Entities;

public class Entity : IEntity
{
  public string ID { get; set; }

  public String GenerateID()
  {
    throw new NotImplementedException();
  }
}
