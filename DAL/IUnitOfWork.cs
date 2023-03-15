namespace DAL;

public interface IUnitOfWork
{
  DBContext Context { get; }
}
