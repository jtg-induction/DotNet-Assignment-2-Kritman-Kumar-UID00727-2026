using System.Threading.Tasks;
using System.Threading;


namespace RestaurantServer.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
    }
}
