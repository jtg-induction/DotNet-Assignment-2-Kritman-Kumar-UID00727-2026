using RestaurantServer.Constants;
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
                throw new ValidationException(ValidationMessages.ReportRequestRequired);
            }

            if (request.TopItems < MinimumTopItems)
            {
                throw new ValidationException(ValidationMessages.InvalidTopItemsCount);
            }

            if (request.TopItems > MaximumTopItems)
            {
                throw new ValidationException(ValidationMessages.InvalidTopItemsCount);
            }

            if (request.StartDate.HasValue && request.EndDate.HasValue &&
                request.StartDate.Value.Date > request.EndDate.Value.Date)
            {
                throw new ValidationException(ValidationMessages.InvalidDateRange);
            }

            if (request.ExcludeItemIds != null &&
                request.ExcludeItemIds.Any(id => id <= 0))
            {
                throw new ValidationException(ValidationMessages.InvalidExcludeItemIds);
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
                throw new ValidationException(ValidationMessages.InvalidRestaurantId);
            }

            if (request == null)
            {
                throw new ValidationException(ValidationMessages.ReportRequestRequired);
            }

            if (request.TopPairs < MinimumTopItems)
            {
                throw new ValidationException(ValidationMessages.InvalidTopPairsCount);
            }

            if (request.TopPairs > MaximumTopItems)
            {
                throw new ValidationException(ValidationMessages.InvalidTopItemsCount);
            }
        }
    }
}
