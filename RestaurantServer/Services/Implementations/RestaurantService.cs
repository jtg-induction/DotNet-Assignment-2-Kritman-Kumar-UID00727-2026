using RestaurantServer.Constants;
using RestaurantServer.DTOs.Responses;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Services.Interfaces;
using RestaurantServer.Validators.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Services.Implementations
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IRestaurantValidator _restaurantValidator;

        public RestaurantService(
            IRestaurantRepository restaurantRepository,
            IRestaurantValidator restaurantValidator)
        {
            _restaurantRepository = restaurantRepository;
            _restaurantValidator = restaurantValidator;
        }

        public async Task<RestaurantListResponse> GetRestaurantsAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            _restaurantValidator.ValidatePagination(page, pageSize);

            var totalRecords = await _restaurantRepository.CountAvailableRestaurantsAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var restaurants = await _restaurantRepository.GetAvailableRestaurantsAsync(page, pageSize, cancellationToken);

            var restaurantDtos = restaurants
                .Select(restaurant => new RestaurantDto(restaurant))
                .ToList();

            var pagination = new PaginationResponse(page, pageSize, totalRecords, totalPages);

            return new RestaurantListResponse(SuccessMessages.RestaurantsRetrieved,
                restaurantDtos, pagination);
        }
    }
}
