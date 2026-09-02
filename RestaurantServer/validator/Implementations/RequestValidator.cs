using RestaurantServer.Constants;
using RestaurantServer.Exceptions;
using RestaurantServer.Validators.Interfaces;

namespace RestaurantServer.Validators.Implementations
{
    /// <summary>
    /// Provides validation for request objects.
    /// </summary>
    public class RequestValidator : IRequestValidator
    {
        /// <summary>
        /// Validates that the specified request is not null.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the request object to validate.
        /// </typeparam>
        /// <param name="request">
        /// The request object to validate.
        /// </param>
        /// <exception cref="ValidationException">
        /// Thrown when the request is null.
        /// </exception>
        public void IsRequestNull<T>(T request)
        {
            if (request == null)
            {
                throw new ValidationException(ErrorMessages.EmptyRequest);
            }
        }
    }
}
