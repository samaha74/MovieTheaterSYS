using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace MovieTheaterSYS.Repository
{
    public interface IRepository<T> where T : class
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        void Delete(T entity);
        EntityEntry<T> Update(T entity);
        Task<EntityEntry<T>> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<T> GetOneAsync(Expression<Func<T, bool>>? where = null, Expression<Func<T, object>>[]? joins = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? where = null, Expression<Func<T, object>>[]? joins = null, CancellationToken cancellationToken = default); 
    }
}
