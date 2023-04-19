namespace DAL.Influx.Samples;

public abstract class Sample
{
  public string Tag { get; set; }

  public DateTime Timestamp { get; set; }

  public object Value { get; set; }

  public Sample()
  {
    Timestamp = DateTime.Now;
  }

  public abstract bool AsBoolean();

  public abstract double AsNumeric();

}
