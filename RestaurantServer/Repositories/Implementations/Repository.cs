using RestaurantServer.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Repositories.Implementations
{
    /// <summary>
    /// Provides a generic implementation of the repository pattern for data access operations 
    /// using Entity Framework.
    /// </summary>
    /// <typeparam name="T">The entity type representing a database table. Must be a reference type.</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="context">The database context used for data operations.</param>
        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Asynchronously retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the found entity, or <c>null</c> if no entity matches the identifier.</returns>
        public async Task<T> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().FindAsync(cancellationToken, new object[] { id });
        }

        /// <summary>
        /// Adds a single entity to the context tracked for insertion.
        /// </summary>
        /// The entity instance to add.</param>
        /// <returns>A completed task representing the tracking operation.</returns>
        public Task Add(T entity)
        {
            _context.Set<T>().Add(entity);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Adds a collection of entities to the context tracked for insertion.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <returns>A completed task representing the tracking operation.</returns>
        public Task AddRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Attaches and marks an existing entity as modified within the context.
        /// </summary>
        /// The entity instance to update.</param>
        public void Update(T entity)
        {
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }


        /// <summary>
        /// Marks an existing entity to be removed from the context.
        /// </summary>
        /// The entity instance to remove.</param>
        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }
    }
}
