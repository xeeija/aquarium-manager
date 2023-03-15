namespace DAL;

public class UnitOfWork : IUnitOfWork
{
  public DBContext Context { get; private set; }

  public UnitOfWork()
  {
    Context = new DBContext();
  }
}
