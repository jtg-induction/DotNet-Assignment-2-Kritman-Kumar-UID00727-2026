using RestaurantServer.Constants;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using RestaurantServer.Enums;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Services.Interfaces;
using RestaurantServer.Validators.Interfaces;
using System;
using System.Runtime.Remoting.Messaging;
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
        private readonly IUserValidator _userValidator;

        public AdminService(
            IRestaurantRepository restaurantRepository,
            IRestaurantOwnerRepository restaurantOwnerRepository,
            IAuthRepository authRepository,
            IUnitOfWork unitOfWork,
            IRestaurantValidator restaurantValidator,
            IUserValidator userValidator)
        {
            _restaurantRepository = restaurantRepository;
            _restaurantOwnerRepository = restaurantOwnerRepository;
            _authRepository = authRepository;
            _unitOfWork = unitOfWork;
            _restaurantValidator = restaurantValidator;
            _userValidator = userValidator;
        }

        public async Task<CreateRestaurantResponse> CreateRestaurantAsync(
            CreateRestaurantRequest request,
            long createdBy,
            CancellationToken cancellationToken = default)
        {
            var restaurant = new Restaurant(request, createdBy);

            await _restaurantRepository.Add(restaurant);

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

            _userValidator.IsUserNullOrDeactivated(user, ValidationMessages.UserNotFound);

            var existingRelationship = await _restaurantOwnerRepository.GetOwnerWithRestaurantIdAsync(restaurantId, user.Id, cancellationToken);

            _restaurantValidator.ValidateOwnerRelationshipDoesNotExist(existingRelationship);

            user.Role = (int)UserRole.Owner;

            var restaurantOwner = new RestaurantOwner(restaurantId, user.Id);

            await _restaurantOwnerRepository.Add(restaurantOwner);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new OnboardRestaurantOwnerResponse(restaurantOwner);
        }
    }
}
