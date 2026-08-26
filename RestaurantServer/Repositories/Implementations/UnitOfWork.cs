using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using RestaurantServer.Repositories.Interfaces;

namespace RestaurantServer.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync(long? personId, CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(personId, cancellationToken);
        }

        public ITransaction BeginTransaction()
        {
            return new DbContextTransactionWrapper(_context.Database.BeginTransaction());
        }

        private class DbContextTransactionWrapper : ITransaction
        {
            private readonly DbContextTransaction _transaction;

            public DbContextTransactionWrapper(DbContextTransaction transaction)
            {
                _transaction = transaction;
            }

            public void Commit()
            {
                _transaction.Commit();
            }

            public void Rollback()
            {
                _transaction.Rollback();
            }

            public void Dispose()
            {
                _transaction.Dispose();
            }
        }
    }
}
