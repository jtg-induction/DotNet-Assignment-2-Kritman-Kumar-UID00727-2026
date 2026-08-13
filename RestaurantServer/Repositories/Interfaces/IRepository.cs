using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(long id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
    }
}
