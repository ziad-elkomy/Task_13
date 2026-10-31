using CinemaSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
namespace CinemaSystem.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context; //= new ApplicationDbContext();
        protected readonly DbSet<T> dbSet;

        public Repository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
            dbSet = _context.Set<T>();
        }
        public int GetCount()
        {
            return dbSet.Count();
        }
        private IQueryable<T> Query(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool isTracked = true
            )
        {
            var entities = dbSet.AsQueryable();
            if (filter != null)
            {
                entities = entities.Where(filter);
            }
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }
            if (!isTracked)
            {
                entities = entities.AsNoTracking();
            }
            return entities;
        }
        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool isTracked = true
            )
        {
            var entities = Query(filter, includes, isTracked);
            return await entities.ToListAsync();
        }
        public async Task<T> GetFirstAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool isTracked = true
            )
        {
            var entities = Query(filter, includes, isTracked);
            return await entities.FirstOrDefaultAsync();
        }
        public async Task<EntityEntry<T>> AddAsync(T entity)
        {
            return await dbSet.AddAsync(entity);
        }
        public void Update(T entity)
        {
            dbSet.Update(entity);
        }
        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }
        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }catch(Exception e)
            {
                throw new Exception("Error In DB");
            }
        }
    }
}
