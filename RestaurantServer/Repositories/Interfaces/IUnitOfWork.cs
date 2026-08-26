using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Interfaces
{
    public interface ITransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }

    public interface IUnitOfWork
    {
        ITransaction BeginTransaction();
        Task SaveChangesAsync(long? personId = null, CancellationToken cancellationToken = default);
    }
}
