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
    /// <summary>
    /// Provides administrative services for creating restaurants and onboarding owners.
    /// </summary>
    public class AdminService : IAdminService
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IRestaurantOwnerRepository _restaurantOwnerRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRestaurantValidator _restaurantValidator;
        private readonly IUserValidator _userValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminService"/> class.
        /// </summary>
        /// <param name="restaurantRepository">The data repository for managing restaurants.</param>
        /// <param name="restaurantOwnerRepository">The data repository for managing restaurant-to-owner relationships.</param>
        /// <param name="authRepository">The data repository for managing user records and login details.</param>
        /// <param name="unitOfWork">The transaction boundary manager for saving multi-repository database state changes.</param>
        /// <param name="restaurantValidator">The validation rules engine for restaurant properties and entities.</param>
        /// <param name="userValidator">The validation rules engine for user record properties and checks.</param>
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

        /// <summary>
        /// Creates a new restaurant and assigns an existing user as its owner.
        /// </summary>
        /// <param name="request">Restaurant and owner details.</param>
        /// <param name="createdBy">ID of the user creating the restaurant.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created restaurant details.</returns>
        /// <exception cref="ValidationException">
        /// Thrown when the owner is invalid or cannot be assigned as an owner.
        /// </exception>
        public async Task<CreateRestaurantResponse> CreateRestaurantAsync(
            CreateRestaurantRequest request,
            long createdBy,
            CancellationToken cancellationToken = default)
        {
            var owner = await _authRepository.GetUserByEmailAsync(request.OwnerEmail, cancellationToken);

            _userValidator.IsUserNullOrDeactivated(owner, ValidationMessages.UserNotFound);

            if (owner.Role == (int)UserRole.Admin)
            {
                throw new ValidationException(ValidationMessages.InvalidRestaurantOwner);
            }

            var restaurant = new Restaurant(request, createdBy);

            await _restaurantRepository.Add(restaurant);

            var restaurantOwner = new RestaurantOwner(restaurant.Id, owner.Id);

            await _restaurantOwnerRepository.Add(restaurantOwner);

            owner.Role = (int)UserRole.Owner;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateRestaurantResponse(restaurant);
        }

        /// <summary>
        /// Onboards users as owners of an existing restaurant.
        /// </summary>
        /// <param name="restaurantId">ID of the restaurant.</param>
        /// <param name="request">Owner email addresses.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The restaurant owner onboarding result.</returns>
        /// <exception cref="ValidationException">
        /// Thrown when the restaurant, email, user, or owner relationship is invalid.
        /// </exception>
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

                _restaurantValidator.ValidateOwnerRelationshipDoesNotExist(existingRelationship);

                users.Add(user);
            }

            var restaurantOwners = new List<RestaurantOwner>();

            foreach (var user in users)
            {
                user.Role = (int)UserRole.Owner;

                var restaurantOwner = new RestaurantOwner(restaurantId, user.Id);

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
