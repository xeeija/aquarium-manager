using DAL.Entities;

namespace DAL.Repository;

public class AquariumItemRepository : Repository<AquariumItem>, IAquariumItemRepository
{
  public AquariumItemRepository(DBContext context) : base(context) { }

  public List<Animal> GetAnimals()
  {
    return FilterByType<Animal>().ToList();
  }

  public List<Coral> GetCorals()
  {
    return FilterByType<Coral>().ToList();
  }

  public IEnumerable<E> FilterByType<E>() where E : AquariumItem
  {
    return FilterBy((_) => true).OfType<E>();

    // var filterType = Expression.TypeIs(Expression.Constant(typeof(E).Name), typeof(E));
    // return FilterBy(Expression.Lambda<Func<AquariumItem, bool>>(filterType, true)).OfType<E>().Cast<E>();

    // var filter = Builders<AquariumItem>.Filter.OfType<E>();
    // return collection.Find(filter).ToEnumerable().Cast<E>();

    // return FilterBy((item) => item.GetType() == typeof(E)).Cast<E>();
  }
}
