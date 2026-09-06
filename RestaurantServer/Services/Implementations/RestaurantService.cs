using RestaurantServer.Constants;
using RestaurantServer.DTOs.Responses;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Services.Interfaces;
using RestaurantServer.validator.Interfaces;
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
        private readonly IItemRepository _itemRepository;
        private readonly IRestaurantValidator _restaurantValidator;
        private readonly IPaginatedValidator _paginatedValidator;

        public RestaurantService(
            IRestaurantRepository restaurantRepository,
            IItemRepository itemRepository,
            IRestaurantValidator restaurantValidator,
            IPaginatedValidator paginatedValidator)
        {
            _restaurantRepository = restaurantRepository;
            _itemRepository = itemRepository;
            _restaurantValidator = restaurantValidator;
            _paginatedValidator = paginatedValidator; 
        }

        public async Task<RestaurantListResponse> GetRestaurantsAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            _paginatedValidator.ValidatePagination(page, pageSize);

            var totalRecords = await _restaurantRepository.CountAvailableRestaurantsAsync(cancellationToken);

            var restaurants = await _restaurantRepository.GetAvailableRestaurantsAsync(page, pageSize,true, cancellationToken);

            var restaurantDtos = restaurants
                .Select(restaurant => new RestaurantDto(restaurant)).ToList();

            var paginatedResult = new PaginatedResponse(page, pageSize, totalRecords);

            return new RestaurantListResponse(restaurantDtos, paginatedResult);
        }

        public async Task<RestaurantItemListResponse> GetRestaurantItemsAsync(
            long restaurantId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            _paginatedValidator.ValidatePagination(page, pageSize);

            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);

            _restaurantValidator.ValidateRestaurantExists(restaurant);

            var totalRecords = await _itemRepository.CountAvailableItemsByRestaurantIdAsync(restaurantId, cancellationToken);

            var items = await _itemRepository.GetAvailableItemsByRestaurantIdAsync(restaurantId, page, pageSize, true, cancellationToken);

            var itemDtos = items
                .Select(item => new ItemDto(item))
                .ToList();

            var paginatedResult = new PaginatedResponse(page, pageSize, totalRecords);

            return new RestaurantItemListResponse(
                restaurantId, itemDtos, paginatedResult);
        }
    }
}
