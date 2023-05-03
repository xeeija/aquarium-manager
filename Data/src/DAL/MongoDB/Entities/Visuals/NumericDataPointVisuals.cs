namespace Context.DAL.Visuals
{
  public class NumericDataPointVisuals : DataPointVisual
  {
    public NumericDataPointVisuals() { }

    public int MinValue { get; set; }

    public int MaxValue { get; set; }

    public string Unit { get; set; }
  }
}
