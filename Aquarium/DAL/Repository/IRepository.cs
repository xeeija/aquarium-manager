using System.Linq.Expressions;
using DAL.Entities;

namespace DAL.Repository;

public interface IRepository<E> where E : Entity
{

  IEnumerable<E> FilterBy(
      Expression<Func<E, bool>> filterFn);

  IEnumerable<TProjected> FilterBy<TProjected>(
      Expression<Func<E, bool>> filterFn,
      Expression<Func<E, TProjected>> projectFn);

  Task<E> FindOneAsync(Expression<Func<E, bool>> filterFn);

  Task<E> FindByIdAsync(string id);

  Task<E> InsertOneAsync(E document);

  Task<E> UpdateOneAsync(E document);

  Task<E> InsertOrUpdateOneAsync(E document);

  Task DeleteOneAsync(Expression<Func<E, bool>> filterFn);

  Task DeleteByIdAsync(string id);

  Task DeleteManyAsync(Expression<Func<E, bool>> filterFn);
}
