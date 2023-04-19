namespace DAL.Entities;

public class Picture : Entity
{
  public string AquariumID { get; set; }

  public string Description { get; set; }

  public string ContentType { get; set; }

  public string PictureID { get; set; }

  public DateTime Uploaded { get; set; } = DateTime.UtcNow;

}
