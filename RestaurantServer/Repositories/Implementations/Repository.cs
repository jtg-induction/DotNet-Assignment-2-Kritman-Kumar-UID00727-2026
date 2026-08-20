using System.Data.Entity;
using System.Threading.Tasks;
using RestaurantServer.Repositories.Interfaces;
using System.Threading;

namespace RestaurantServer.Repositories.Implementations
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().FindAsync(cancellationToken, new object[] { id });
        }

        public Task Add(T entity)
        {
            _context.Set<T>().Add(entity);
            return Task.CompletedTask;
        }

        public void Update(T entity)
        {
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }
    }
}
