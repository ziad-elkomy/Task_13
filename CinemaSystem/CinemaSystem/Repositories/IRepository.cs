using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace CinemaSystem.Repositories
{
    public interface IRepository<T> where T : class
    {
        public int GetCount();
        public Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool isTracked = true
            );
        public Task<T> GetFirstAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool isTracked = true
            );
        public Task<EntityEntry<T>> AddAsync(T entity);
        public void Update(T entity);
        public void Remove(T entity);
        public Task<int> SaveChangesAsync();
    }
}
