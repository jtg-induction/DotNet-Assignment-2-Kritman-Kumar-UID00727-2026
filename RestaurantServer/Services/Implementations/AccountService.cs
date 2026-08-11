using RestaurantServer.Constants;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using RestaurantServer.Exceptions;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace RestaurantServer.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository; 
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountService"/> class.
        /// </summary>
        /// <param name="accountRepository">
        /// The repository used to access and update user account data.
        /// </param>
        /// <param name="refreshTokenRepository">
        /// The repository used to manage the user's refresh tokens.
        /// </param>
        public AccountService(IAccountRepository accountRepository,
                IRefreshTokenRepository refreshTokenRepository)
        {
            _accountRepository = accountRepository;
            _refreshTokenRepository = refreshTokenRepository;
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
        /// <exception cref="BusinessException">
        /// Thrown when the specified user does not exist.
        /// </exception>
        public async Task<UpdateAccountResponse> UpdateAccountAsync(
            long userId,
            UpdateAccountRequest request)
        {
            var user = await _accountRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new BusinessException(
                    ErrorMessages.NotFound);
            }

            user.Name = request.Name.Trim();
            user.MobileNumber = request.MobileNumber;
            user.UpdatedAt = DateTime.UtcNow;

            return new UpdateAccountResponse
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                MobileNumber = user.MobileNumber,
                Message = SuccessMessages.AccountUpdateSuccessful
            };
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
        /// <exception cref="BusinessException">
        /// Thrown when the specified user does not exist or the account
        /// is already inactive.
        /// </exception>
        public async Task<string> DeactivateAccountAsync(long userId)
        {
            var user = await _accountRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new BusinessException(
                    ValidationMessages.UserNotFound);
            }

            if (!user.IsActive)
            {
                throw new BusinessException(
                    ValidationMessages.UserInactive);
            }

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _refreshTokenRepository.RevokeAllByUserIdAsync(userId);
            await _accountRepository.SaveAsync();

            return SuccessMessages.AccountDeactivatedSuccessful;
        }

    }
}
