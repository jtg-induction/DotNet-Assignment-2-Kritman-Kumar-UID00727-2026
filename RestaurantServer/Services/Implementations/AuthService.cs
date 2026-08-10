using System;
using RestaurantServer.Constants;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using RestaurantServer.Enums;
using RestaurantServer.Exceptions;
using RestaurantServer.Helpers.Interfaces;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces; 
using System.Threading.Tasks; 

namespace RestaurantServer.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(
             IAuthRepository authRepository,
             IRefreshTokenRepository refreshTokenRepository,
             IPasswordHasher passwordHasher,
             IJwtTokenService jwtTokenService)
        {
            _authRepository = authRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<SignupResponse> SignupAsync(SignupRequest request)
        { 
            request.Email = request.Email.Trim().ToLowerInvariant();
             
            var existingUser = await _authRepository.GetUserByEmailAsync(request.Email);

            if (existingUser != null)
            {
                throw new BusinessException(ValidationMessages.EmailAlreadyExists);
            }
             
            var passwordHash = _passwordHasher.HashPassword(request.Password);
             
            var user = new User
            {
                Name = request.Name.Trim(),
                Email = request.Email,
                PasswordHash = passwordHash,
                Balance = 1000m,
                Role = (int)UserRole.Customer,
                IsActive = true,
                MobileNumber = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
             
            _authRepository.AddUser(user);
            await _authRepository.SaveAsync();
             
            return new SignupResponse
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Message = SuccessMessages.UserRegistered
            };
        }

        public async Task<LoginResult> LoginAsync(LoginRequest request)
        {
            request.Email = request.Email.Trim().ToLowerInvariant();

            var user = await _authRepository.GetUserByEmailAsync(request.Email);

            if (user == null)
            {
                throw new BusinessException(
                    ValidationMessages.InvalidCredentials);
            }

            if (!user.IsActive)
            {
                throw new BusinessException(
                    ValidationMessages.UserInactive);
            }

            if (!_passwordHasher.VerifyPassword(
                request.Password,
                user.PasswordHash))
            {
                throw new BusinessException(
                    ValidationMessages.InvalidCredentials);
            }

            var accessToken = _jwtTokenService.GenerateAccessToken(user);

            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            var now = DateTime.UtcNow;

            await _refreshTokenRepository.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                IsRevoked = false,
                CreatedAt = now,
                UpdatedAt = now,
                ExpiresAt = now.AddDays(30)
            });

            await _refreshTokenRepository.SaveAsync();

            return new LoginResult
            {
                Response = new LoginResponse
                {
                    AccessToken = accessToken,
                    UserId = user.Id,
                    Name = user.Name,
                    Role = (UserRole)user.Role,
                    Message = SuccessMessages.LoginSuccessful
                },
                RefreshToken = refreshToken
            };
        }

        public async Task<LoginResult> RefreshTokenAsync(string refreshToken)
        {
            var existingRefreshToken =
                await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (existingRefreshToken == null)
            {
                throw new BusinessException(
                    ValidationMessages.InvalidRefreshToken);
            }

            if (existingRefreshToken.IsRevoked)
            {
                throw new BusinessException(
                    ValidationMessages.InvalidRefreshToken);
            }

            if (existingRefreshToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new BusinessException(
                    ValidationMessages.InvalidRefreshToken);
            }

            var user = await _authRepository
                .GetUserByIdAsync(existingRefreshToken.UserId);

            if (user == null || !user.IsActive)
            {
                throw new BusinessException(
                    ValidationMessages.InvalidRefreshToken);
            }

            var accessToken = _jwtTokenService.GenerateAccessToken(user);

            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

            existingRefreshToken.Token = newRefreshToken;
            existingRefreshToken.UpdatedAt = DateTime.UtcNow;
            existingRefreshToken.ExpiresAt =
                DateTime.UtcNow.AddDays(30);
            existingRefreshToken.IsRevoked = false;

            _refreshTokenRepository.Update(existingRefreshToken);

            await _refreshTokenRepository.SaveAsync();

            return new LoginResult
            {
                Response = new LoginResponse
                {
                    AccessToken = accessToken,
                    UserId = user.Id,
                    Name = user.Name,
                    Role = (UserRole)user.Role,
                    Message = SuccessMessages.TokenRefreshed
                },
                RefreshToken = newRefreshToken
            };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new BusinessException(
                    ValidationMessages.InvalidRefreshToken);
            }

            var existingRefreshToken =
                await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (existingRefreshToken == null ||
                existingRefreshToken.IsRevoked)
            {
                throw new BusinessException(
                    ValidationMessages.InvalidRefreshToken);
            }

            existingRefreshToken.IsRevoked = true;
            existingRefreshToken.UpdatedAt = DateTime.UtcNow;

            _refreshTokenRepository.Update(existingRefreshToken);

            await _refreshTokenRepository.SaveAsync();
        }

    }
}
    