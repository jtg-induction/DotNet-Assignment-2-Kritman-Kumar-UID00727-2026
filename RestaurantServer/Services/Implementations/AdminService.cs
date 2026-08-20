using RestaurantServer.Constants;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using RestaurantServer.Enums;
using RestaurantServer.Exceptions;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Services.Interfaces;
using RestaurantServer.Validators.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RestaurantServer.Services.Implementations
{
    public class AdminService : IAdminService
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
            var owner = await _authRepository.GetUserByEmailAsync(
                request.OwnerEmail,
                cancellationToken);

            _userValidator.IsUserNullOrDeactivated(
                owner,
                ValidationMessages.UserNotFound);

            if (owner.Role == (int)UserRole.Admin)
            {
                throw new ValidationException(
                    ValidationMessages.InvalidRestaurantOwner);
            }

            var restaurant = new Restaurant(request, createdBy);

            await _restaurantRepository.Add(restaurant);

            var restaurantOwner = new RestaurantOwner(
                restaurant.Id,
                owner.Id);

            await _restaurantOwnerRepository.Add(restaurantOwner);

            owner.Role = (int)UserRole.Owner;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateRestaurantResponse(restaurant);
        }

        public async Task<RestaurantOwnerResult> OnboardRestaurantOwnerAsync(
            long restaurantId,
            OnboardRestaurantOwnerRequest request,
            CancellationToken cancellationToken = default)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);

            _restaurantValidator.ValidateRestaurantExists(restaurant);

            var users = new List<User>();
            var emailSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var email in request.Emails)
            {
                if (string.IsNullOrWhiteSpace(email) ||
                    !Regex.IsMatch(email.Trim(), ValidationConstants.EmailRegex))
                {
                    throw new ValidationException(ValidationMessages.InvalidEmail);
                }

                var normalizedEmail = email.Trim();

                if (!emailSet.Add(normalizedEmail))
                {
                    throw new ValidationException(ValidationMessages.DuplicateOwnerEmail);
                }

                var user = await _authRepository.GetUserByEmailAsync(normalizedEmail, cancellationToken);

                _userValidator.IsUserNullOrDeactivated(user, ValidationMessages.UserNotFound);

                if (user.Role == (int)UserRole.Admin)
                {
                    throw new ValidationException(ValidationMessages.InvalidRestaurantOwner);
                }

                var existingRelationship = await _restaurantOwnerRepository
                    .GetOwnerWithRestaurantIdAsync(restaurantId, user.Id, cancellationToken);

                _restaurantValidator.ValidateOwnerRelationshipDoesNotExist(
                    existingRelationship);

                users.Add(user);
            }

            var restaurantOwners = new List<RestaurantOwner>();

            foreach (var user in users)
            {
                user.Role = (int)UserRole.Owner;

                var restaurantOwner = new RestaurantOwner(
                    restaurantId,
                    user.Id);

                await _restaurantOwnerRepository.Add(restaurantOwner);

                restaurantOwners.Add(restaurantOwner);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RestaurantOwnerResult
            {
                RestaurantId = restaurantId,
                Message = SuccessMessages.ownersOnboardedSuccessful,
                Owners = restaurantOwners
            .Select((restaurantOwner, index) =>
                new OnboardRestaurantOwnerResponse(restaurantOwner)).ToList()
            };
        }
    }
}
