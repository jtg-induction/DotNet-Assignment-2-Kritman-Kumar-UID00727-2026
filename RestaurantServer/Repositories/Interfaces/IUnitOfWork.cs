using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync(long? personId = null, CancellationToken cancellationToken = default);
    }
}
