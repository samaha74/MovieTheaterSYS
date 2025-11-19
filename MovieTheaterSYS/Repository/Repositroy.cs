
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MovieTheaterSYS.DataAccess;
using NuGet.Protocol.Core.Types;
using System.Linq.Expressions;

namespace MovieTheaterSYS.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        ApplicationDbcontext _db;

        public Repository(ApplicationDbcontext db)
        {
            _db = db;
        }

        public async Task<EntityEntry<T>> AddAsync(T entity , CancellationToken cancellationToken = default)
        {
            return await _db.Set<T>().AddAsync(entity , cancellationToken);  
        }

        public void Delete(T entity)
        {
            _db.Set<T>().Remove(entity);
        }
        public EntityEntry<T> Update(T entity)
        {
           return _db.Set<T>().Update(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? where = null , Expression<Func<T, object>>[]? joins = null, CancellationToken cancellationToken = default)
        {
            var entities = _db.Set<T>().AsQueryable();
            if(where!=null)
            {
                               entities = entities.Where(where);
            }
            if(joins!=null)
            {
                foreach(var join in joins)
                {
                    entities = entities.Include(join);
                }
            }
            return await entities.ToListAsync(cancellationToken);
        }

        public async Task<T> GetOneAsync(Expression<Func<T, bool>>? where = null, Expression<Func<T, object>>[]? joins = null , CancellationToken cancellationToken = default)
        {
            var entities = (await GetAllAsync(where, joins , cancellationToken)).FirstOrDefault();
            return entities;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }

    }
}
