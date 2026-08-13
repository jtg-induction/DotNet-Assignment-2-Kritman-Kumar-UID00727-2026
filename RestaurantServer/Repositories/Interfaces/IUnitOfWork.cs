using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
