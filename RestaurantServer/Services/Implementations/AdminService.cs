using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using RestaurantServer.Enums;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Services.Interfaces;
using RestaurantServer.Validators.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Services.Implementations
{
    public class AdminService : IRestaurantService
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IRestaurantOwnerRepository _restaurantOwnerRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRestaurantValidator _restaurantValidator;

        public AdminService(
            IRestaurantRepository restaurantRepository,
            IRestaurantOwnerRepository restaurantOwnerRepository,
            IAuthRepository authRepository,
            IUnitOfWork unitOfWork,
            IRestaurantValidator restaurantValidator)
        {
            _restaurantRepository = restaurantRepository;
            _restaurantOwnerRepository = restaurantOwnerRepository;
            _authRepository = authRepository;
            _unitOfWork = unitOfWork;
            _restaurantValidator = restaurantValidator;
        }

        public async Task<CreateRestaurantResponse> CreateRestaurantAsync(
            CreateRestaurantRequest request,
            long createdBy,
            CancellationToken cancellationToken = default)
        {
            var restaurant = new Restaurant(request, createdBy);

            await _restaurantRepository.AddAsync(restaurant, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateRestaurantResponse(restaurant);
        }

        public async Task<OnboardRestaurantOwnerResponse> OnboardRestaurantOwnerAsync(long restaurantId,
            OnboardRestaurantOwnerRequest request,
            CancellationToken cancellationToken = default)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);

            _restaurantValidator.ValidateRestaurantExists(restaurant);

            var user = await _authRepository.GetUserByEmailAsync(request.Email);

            _restaurantValidator.ValidateUserCanBeOwner(user);

            var existingRelationship = await _restaurantOwnerRepository.GetAsync(restaurantId, user.Id, cancellationToken);

            _restaurantValidator.ValidateOwnerRelationshipDoesNotExist(existingRelationship);

            user.Role = (int)UserRole.Owner;
            user.UpdatedAt = DateTime.UtcNow;

            var restaurantOwner = new RestaurantOwner(restaurantId, user.Id);

            await _restaurantOwnerRepository.AddAsync(restaurantOwner, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new OnboardRestaurantOwnerResponse(restaurantOwner);
        }
    }
}
