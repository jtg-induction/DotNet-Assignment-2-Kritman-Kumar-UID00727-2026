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
        private readonly IItemRepository _itemRepository;
        private readonly IRestaurantValidator _restaurantValidator;

        public RestaurantService(
            IRestaurantRepository restaurantRepository,
            IItemRepository itemRepository,
            IRestaurantValidator restaurantValidator)
        {
            _restaurantRepository = restaurantRepository;
            _itemRepository = itemRepository;
            _restaurantValidator = restaurantValidator;
        }

        public async Task<RestaurantListResponse> GetRestaurantsAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            _restaurantValidator.ValidatePagination(page, pageSize);

            var totalRecords = await _restaurantRepository.CountAvailableRestaurantsAsync(cancellationToken);

            var restaurants = await _restaurantRepository.GetAvailableRestaurantsAsync(page, pageSize, cancellationToken);

            var restaurantDtos = restaurants
                .Select(restaurant => new RestaurantDto(restaurant)).ToList();

            var pagination = new PaginationResponse(page, pageSize, totalRecords);

            return new RestaurantListResponse(SuccessMessages.RestaurantsRetrieved,
                restaurantDtos, pagination);
        }

        public async Task<RestaurantItemListResponse> GetRestaurantItemsAsync(
            long restaurantId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            _restaurantValidator.ValidatePagination(page, pageSize);

            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);

            _restaurantValidator.ValidateRestaurantExists(restaurant);

            var totalRecords = await _itemRepository.CountAvailableItemsByRestaurantIdAsync(restaurantId, cancellationToken);

            var items = await _itemRepository.GetAvailableItemsByRestaurantIdAsync(restaurantId, page, pageSize, cancellationToken);

            var itemDtos = items
                .Select(item => new ItemDto(item))
                .ToList();

            var pagination = new PaginationResponse(page, pageSize, totalRecords);

            return new RestaurantItemListResponse(
                SuccessMessages.MenuItemsRetrieved,
                restaurantId, itemDtos, pagination);
        }
    }
}
