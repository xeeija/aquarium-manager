namespace DAL.Influx.Samples;

public class BinarySample : Sample
{
  public override bool AsBoolean() => Convert.ToBoolean(Value);

  public override double AsNumeric() => Convert.ToDouble(Value);
}
