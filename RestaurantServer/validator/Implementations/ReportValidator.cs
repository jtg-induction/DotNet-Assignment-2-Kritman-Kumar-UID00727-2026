using RestaurantServer.DTOs.Requests;
using RestaurantServer.Exceptions;
using RestaurantServer.Validators.Interfaces;
using System.Linq;

namespace RestaurantServer.Validators.Implementations
{
    /// <summary>
    /// Provides validation logic for report requests.
    /// </summary>
    public class ReportValidator : IReportValidator
    {
        private const int MinimumTopItems = 1;
        private const int MaximumTopItems = 100;

        /// <summary>
        /// Validates the request parameters for the top ordered items report.
        /// </summary>
        /// <param name="request">
        /// The top ordered items report request.
        /// </param>
        /// <exception cref="ValidationException">
        /// Thrown when the request contains invalid parameters.
        /// </exception>
        public void ValidateTopOrderedItemsRequest(
            TopOrderedItemsRequest request)
        {
            if (request == null)
            {
                throw new ValidationException("Report request cannot be null.");
            }

            if (request.TopItems < MinimumTopItems)
            {
                throw new ValidationException("TopItems must be greater than 0.");
            }

            if (request.TopItems > MaximumTopItems)
            {
                throw new ValidationException("TopItems cannot be greater than 100.");
            }

            if (request.StartDate.HasValue &&
                request.EndDate.HasValue &&
                request.StartDate.Value.Date > request.EndDate.Value.Date)
            {
                throw new ValidationException("StartDate cannot be later than EndDate.");
            }

            if (request.ExcludeItemIds != null &&
                request.ExcludeItemIds.Any(id => id <= 0))
            {
                throw new ValidationException("ExcludeItemIds must contain only valid item IDs.");
            }
        }

        /// <summary>
        /// Validates the request parameters for the frequently
        /// bought together report.
        /// </summary>
        /// <param name="restaurantId">
        /// The identifier of the restaurant.
        /// </param>
        /// <param name="request">
        /// The frequently bought together report request.
        /// </param>
        /// <exception cref="ValidationException">
        /// Thrown when the request contains invalid parameters.
        /// </exception>
        public void ValidateFrequentlyBoughtTogetherRequest(
            long restaurantId,
            FrequentlyBoughtTogetherRequest request)
        {
            if (restaurantId <= 0)
            {
                throw new ValidationException("RestaurantId must be greater than 0.");
            }

            if (request == null)
            {
                throw new ValidationException("Report request cannot be null.");
            }

            if (request.TopPairs < MinimumTopItems)
            {
                throw new ValidationException("TopPairs must be greater than 0.");
            }

            if (request.TopPairs > MaximumTopItems)
            {
                throw new ValidationException("TopPairs cannot be greater than 100.");
            }
        }
    }
}
