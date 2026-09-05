using RestaurantServer.Constants;
using RestaurantServer.validator.Interfaces;
using System.ComponentModel.DataAnnotations; 

namespace RestaurantServer.validator.Implementations
{
    public class PaginatedValidator: IPaginatedValidator
    {

        /// <summary>
        /// Validates pagination parameters.
        /// </summary>
        /// <param name="page">The current page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <exception cref="ValidationException">Thrown when page or pageSize is less than 1.</exception>
        public void ValidatePagination(int page, int pageSize)
        {
            if (page < 1)
            {
                throw new ValidationException(ValidationMessages.InvalidPageNumber);
            }

            if (pageSize < 1)
            {
                throw new ValidationException(ValidationMessages.InvalidPageSize);
            }
        }
    }
}
