using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using RestaurantServer.Repositories.Interfaces;

namespace RestaurantServer.Repositories.Implementations
{
    /// <summary>
    /// Provides transaction and persistence management for the application database context.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Persists all pending changes to the database asynchronously.
        /// </summary>
        /// <param name="personId">
        /// The identifier of the person performing the operation, if available.
        /// </param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        public async Task SaveChangesAsync(long? personId, CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(personId, cancellationToken);
        }

        /// <summary>
        /// Begins a new database transaction.
        /// </summary>
        /// <returns>A transaction wrapper that can be committed, rolled back, or disposed.</returns>
        public ITransaction BeginTransaction()
        {
            return new DbContextTransactionWrapper(_context.Database.BeginTransaction());
        }

        private class DbContextTransactionWrapper : ITransaction
        {
            private readonly DbContextTransaction _transaction;

            /// <summary>
            /// Provides an implementation of <see cref="ITransaction"/> that wraps
            /// the Entity Framework database transaction.
            /// </summary>
            public DbContextTransactionWrapper(DbContextTransaction transaction)
            {
                _transaction = transaction;
            }

            /// <summary>
            /// Commits the current database transaction.
            /// </summary>
            public void Commit()
            {
                _transaction.Commit();
            }

            /// <summary>
            /// Rolls back the current database transaction.
            /// </summary>
            public void Rollback()
            {
                _transaction.Rollback();
            }

            /// <summary>
            /// Releases the resources used by the database transaction.
            /// </summary>
            public void Dispose()
            {
                _transaction.Dispose();
            }
        }
    }
}
