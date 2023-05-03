namespace Context.DAL.Visuals
{
  public class BinaryDataPointVisuals : DataPointVisual
  {
    public BinaryDataPointVisuals() { }

    public List<BinaryValueMapping> ValueMapping { get; set; } = new();
  }
}
