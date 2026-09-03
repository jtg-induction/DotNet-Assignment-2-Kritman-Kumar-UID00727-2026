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

namespace RestaurantServer.Services.Implementations
{
    /// <summary>
    /// Provides administrative services for creating restaurants and onboarding owners.
    /// </summary>
    public class RestaurantAdminService : IRestaurantAdminService
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IRestaurantOwnerRepository _restaurantOwnerRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRestaurantValidator _restaurantValidator;
        private readonly IUserValidator _userValidator;
        private readonly IUserSessionService _userSessionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestaurantAdminService"/> class.
        /// </summary>
        /// <param name="restaurantRepository">The data repository for managing restaurants.</param>
        /// <param name="restaurantOwnerRepository">The data repository for managing restaurant-to-owner relationships.</param>
        /// <param name="authRepository">The data repository for managing user records and login details.</param>
        /// <param name="unitOfWork">The transaction boundary manager for saving multi-repository database state changes.</param>
        /// <param name="restaurantValidator">The validation rules engine for restaurant properties and entities.</param>
        /// <param name="userValidator">The validation rules engine for user record properties and checks.</param>
        public RestaurantAdminService(
            IRestaurantRepository restaurantRepository,
            IRestaurantOwnerRepository restaurantOwnerRepository,
            IUsersRepository usersRepository,
            IUnitOfWork unitOfWork,
            IRestaurantValidator restaurantValidator,
            IUserValidator userValidator,
            IUserSessionService userSessionService)
        {
            _restaurantRepository = restaurantRepository;
            _restaurantOwnerRepository = restaurantOwnerRepository;
            _usersRepository = usersRepository;
            _unitOfWork = unitOfWork;
            _restaurantValidator = restaurantValidator;
            _userValidator = userValidator;
            _userSessionService = userSessionService;
        }

        private async Task<List<RestaurantOwner>> OnboardOwnerAsync(Restaurant restaurant, List<string> emails,
            bool isOnboard = true, CancellationToken cancellationToken = default)
        {

            var emailSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string email in emails)
            {
                emailSet.Add(email.Trim());
            }

            List<User> users = await _usersRepository.GetUsersByEmailsAsync(emailSet.ToList(), cancellationToken);

            var usersByEmail = users.ToDictionary(user => user.Email.Trim(), StringComparer.OrdinalIgnoreCase);

            foreach (var email in emailSet)
            {
                if (!usersByEmail.TryGetValue(email, out var user))
                {
                    throw new ValidationException(string.Format(ErrorMessages.UserNotFound, email));
                }

                _userValidator.IsUserNullOrDeactivated(user, string.Format(ErrorMessages.UserNotFound, user.Email));
                _restaurantValidator.ValidateAdminRole(user);
            }

            if (isOnboard)
            {
                var userIds = users.Select(user => user.Id).ToList();

                var existingOwners = await _restaurantOwnerRepository
                    .GetOwnersByRestaurantAndUserIdsAsync(restaurant.Id, userIds, true, cancellationToken);

                var existingOwnerUserIds = existingOwners
                    .Select(owner => owner.UserId).ToHashSet();

                foreach (var user in users)
                {
                    if (existingOwnerUserIds.Contains(user.Id))
                    {
                        throw new ValidationException(ErrorMessages.OwnerRelationshipAlreadyExists);
                    }
                }
            }

            var restaurantOwners = users.Select(
                user =>
                {
                    user.Role = (int)UserRole.Owner;
                    return new RestaurantOwner(restaurant, user);
                }).ToList();

            await _restaurantOwnerRepository.AddRange(restaurantOwners);

            return restaurantOwners;
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
            CancellationToken cancellationToken = default)
        {

            var createdBy = _userSessionService.GetUserId().Value;

            await _restaurantValidator.ValidateMobileNumber(request.MobileNumber);

            var restaurant = new Restaurant(request);

            await _restaurantRepository.Add(restaurant);

            List<RestaurantOwner> restaurantOwners = await this.OnboardOwnerAsync(restaurant, request.OwnersEmails, isOnboard: false, cancellationToken);

            await _unitOfWork.SaveChangesAsync(createdBy, cancellationToken);

            List<OwnerDto> owners = restaurantOwners
                    .Select(restaurantOwner => new OwnerDto(restaurantOwner))
                    .ToList();

            return new CreateRestaurantResponse(restaurant, owners);
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
        public async Task<OnboardRestaurantResponses> OnboardRestaurantOwnerAsync(
            long restaurantId, OnboardRestaurantOwnerRequest request,
            CancellationToken cancellationToken = default)
        {

            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);

            _restaurantValidator.ValidateRestaurantExists(restaurant);

            List<RestaurantOwner> restaurantOwners = await this.OnboardOwnerAsync(restaurant, request.Emails, isOnboard: true, cancellationToken);

            await _unitOfWork.SaveChangesAsync(personId: null, cancellationToken);

            return new OnboardRestaurantResponses
            {
                RestaurantId = restaurantId,
                Message = SuccessMessages.ownersOnboardedSuccessful,
                Owners = restaurantOwners
                    .Select(restaurantOwner => new OwnerDto(restaurantOwner))
                    .ToList()
            };
        }

    }
}
