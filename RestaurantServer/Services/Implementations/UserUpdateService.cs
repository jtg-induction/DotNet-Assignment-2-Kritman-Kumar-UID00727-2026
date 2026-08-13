using RestaurantServer.Constants;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using RestaurantServer.Exceptions;
using RestaurantServer.Repositories.Implementations;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Services.Interfaces;
using RestaurantServer.Validators.Interfaces;
using System;
using System.Threading.Tasks;

namespace RestaurantServer.Services.Implementations
{
    public class UserUpdateService : IUserUpdateService
    {
        private readonly IUsersRepository _usersRepository; 
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserValidator _userValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserUpdateService"/> class.
        /// </summary>
        /// <param name="accountRepository">
        /// The repository used to access and update user account data.
        /// </param>
        /// <param name="refreshTokenRepository">
        /// The repository used to manage the user's refresh tokens.
        /// </param>
        public UserUpdateService(IUsersRepository usersRepository,
                IRefreshTokenRepository refreshTokenRepository,
                IUnitOfWork unitOfWork,
                IUserValidator userValidator)
        {
            _usersRepository = usersRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _userValidator = userValidator;
        }

        /// <summary>
        /// Updates the account details of the specified user.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user whose account should be updated.
        /// </param>
        /// <param name="request">
        /// The account details to update.
        /// </param>
        /// <returns>
        /// A response containing the updated account information.
        /// </returns>
        /// <exception cref="ValidationException">
        /// Thrown when the specified user does not exist.
        /// </exception>
        public async Task<UpdateUserResponse> UpdateAccountAsync(
            long userId,
            UpdateAccountRequest request)
        { 
            var user = await _usersRepository.GetByIdAsync(userId);

            _userValidator.ValidateUserExists(user);

            user.Name = request.Name.Trim();
            user.MobileNumber = request.MobileNumber;
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return new UpdateUserResponse(user);
        }

        /// <summary>
        /// Deactivates the specified user's account and revokes
        /// all active refresh tokens associated with the user.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user whose account should be deactivated.
        /// </param>
        /// <returns>
        /// A success message confirming that the account was deactivated.
        /// </returns>
        /// <exception cref="ValidationException">
        /// Thrown when the specified user does not exist or the account
        /// is already inactive.
        /// </exception>
        public async Task<string> DeactivateAccountAsync(long userId)
        {
            var user = await _usersRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new ValidationException(ValidationMessages.UserNotFound);
            }

            if (!user.IsActive)
            {
                throw new ValidationException(
                    ValidationMessages.UserInactive);
            }

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _refreshTokenRepository.RevokeAllByUserIdAsync(userId);
            await _unitOfWork.SaveChangesAsync();

            return SuccessMessages.AccountDeactivatedSuccessful;
        }

    }
}
