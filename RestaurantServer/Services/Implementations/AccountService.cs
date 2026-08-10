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

        public AccountService(IAccountRepository accountRepository,
                IRefreshTokenRepository refreshTokenRepository)
        {
            _accountRepository = accountRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }

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