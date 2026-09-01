using RestaurantServer.Constants;
using RestaurantServer.DTOs.Requests;
using RestaurantServer.DTOs.Responses;
using RestaurantServer.Exceptions;
using RestaurantServer.Helpers.Interfaces;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.validator.Interfaces;
using RestaurantServer.Validators.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationValidator _authenticationValidator;
        private readonly IRefreshTokenValidator _refreshTokenValidator;
        private readonly IUserValidator _userValidator;
        private readonly IRequestValidator _requestValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthService"/> class.
        /// </summary>
        /// <param name="authRepository">
        /// The repository used to retrieve and manage user authentication data.
        /// </param>
        /// <param name="refreshTokenRepository">
        /// The repository used to create, retrieve, update, and manage refresh tokens.
        /// </param>
        /// <param name="passwordHasher">
        /// The service used to securely hash and verify user passwords.
        /// </param>
        /// <param name="jwtTokenService">
        /// The service used to generate access tokens and refresh tokens.
        /// </param>
        /// <param name="unitOfWork">
        /// The unit of work used to persist authentication-related changes to the database.
        /// </param>
        /// <param name="authenticationValidator">
        /// The validator used to validate users, credentials, and refresh tokens.
        /// </param>
        public AuthService(
             IUsersRepository usersRepository,
             IRefreshTokenRepository refreshTokenRepository,
             IPasswordHasher passwordHasher,
             IJwtTokenService jwtTokenService,
             IUnitOfWork unitOfWork,
             IAuthenticationValidator authenticationValidator,
            IRefreshTokenValidator refreshTokenValidator,
            IUserValidator userValidator,
            IRequestValidator requestValidator)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _unitOfWork = unitOfWork;
            _authenticationValidator = authenticationValidator;
            _refreshTokenValidator = refreshTokenValidator;
            _userValidator = userValidator;
            _requestValidator = requestValidator;
            _usersRepository = usersRepository;
        }

        /// <summary>
        /// Registers a new customer account by validating the email address,
        /// securely hashing the password, and storing the new user.
        /// </summary>
        /// <param name="request">
        /// The user registration details containing the name, email, and password.
        /// </param>
        /// <returns>
        /// A response containing the newly registered user's information.
        /// </returns>
        /// <exception cref="ValidationException">
        /// Thrown when an account with the provided email address already exists.
        /// </exception>
        public async Task<SignupResponse> SignupAsync(SignupRequest request,
            CancellationToken cancellationToken = default)
        {
            _requestValidator.IsRequestNull(request);

            request.Email = request.Email.Trim().ToLowerInvariant();

            var existingUser = await _usersRepository.GetUserByEmailAsync(request.Email);

            if (existingUser != null)
            {
                throw new ValidationException(ValidationMessages.EmailAlreadyExists);
            }

            var passwordHash = _passwordHasher.HashPassword(request.Password);
            var user = new User(request.Name.Trim(), request.Email, passwordHash);

            await _usersRepository.Add(user);
            await _unitOfWork.SaveChangesAsync(personId: null, cancellationToken);

            return new SignupResponse(user);
        }

        /// <summary>
        /// Authenticates a user using the provided email and password,
        /// and generates a new access token and refresh token upon successful authentication.
        /// </summary>
        /// <param name="request">
        /// The user's login credentials containing the email and password.
        /// </param>
        /// <returns>
        /// A login result containing the authentication response and refresh token.
        /// </returns>
        /// <exception cref="ValidationException">
        /// Thrown when the user does not exist, the account is inactive,
        /// or the provided password is invalid.
        /// </exception> 
        public async Task<LoginResult> LoginAsync(LoginRequest request,
            CancellationToken cancellationToken = default)
        {
            _requestValidator.IsRequestNull(request);

            request.Email = request.Email.Trim().ToLowerInvariant();

            var user = await _usersRepository.GetUserByEmailAsync(request.Email);

            if (user == null)
            {
                throw new ValidationException(
                    ValidationMessages.InvalidCredentials);
            }

            _authenticationValidator.ValidateUserIsActive(user);

            var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);

            _authenticationValidator.ValidatePassword(isPasswordValid);

            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            await _refreshTokenRepository.Add(
                new RefreshToken(user.Id)
                {
                    Token = refreshToken
                });

            await _unitOfWork.SaveChangesAsync(personId: null, cancellationToken);

            return new LoginResult
            {
                Response = new LoginResponse(user, accessToken, SuccessMessages.LoginSuccessful),
                RefreshToken = refreshToken
            };
        }

        /// <summary>
        /// Validates the existing refresh token and the associated user,
        /// then generates and persists a new access token and refresh token.
        /// </summary>
        /// <param name="refreshToken">
        /// The refresh token used to obtain a new authentication token pair.
        /// </param>
        /// <returns>
        /// A refresh result containing the new access token response and refresh token.
        /// </returns>
        /// <exception cref="ValidationException">
        /// Thrown when the refresh token is invalid, revoked, expired,
        /// or associated with a nonexistent or inactive user.
        /// </exception>
        public async Task<RefreshResult> RefreshTokenAsync(string refreshToken,
            CancellationToken cancellationToken = default)
        {
            var existingRefreshToken =
                await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            _refreshTokenValidator.ValidateRefreshTokenIsValid(existingRefreshToken);

            var user = await _usersRepository.GetByIdAsync(existingRefreshToken.UserId);

            _userValidator.IsUserNullOrDeactivated(user);

            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

            existingRefreshToken.Token = newRefreshToken;
            existingRefreshToken.ExpiresAt = DateTime.UtcNow.AddDays(CookieConstants.ExpiresAtInDays);
            existingRefreshToken.IsRevoked = false;

            _refreshTokenRepository.Update(existingRefreshToken);

            await _unitOfWork.SaveChangesAsync(personId: null, cancellationToken);

            var response = new RefreshResponse(accessToken, "Bearer");

            return new RefreshResult
            {
                Response = response,
                RefreshToken = newRefreshToken
            };
        }

        /// <summary>
        /// Logs out the user by validating and revoking the specified refresh token.
        /// </summary>
        /// <param name="refreshToken">
        /// The refresh token that identifies the user's active authentication session.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous logout operation.
        /// </returns>
        /// <exception cref="ValidationException">
        /// Thrown when the refresh token is missing, invalid, or has already been revoked.
        /// </exception>
        public async Task LogoutAsync(string refreshToken,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ValidationException(
                    ValidationMessages.InvalidRefreshToken);
            }

            var existingRefreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            _refreshTokenValidator.ValidateRefreshToken(existingRefreshToken);
            _refreshTokenValidator.ValidateRefreshTokenIsNotRevoked(existingRefreshToken);

            existingRefreshToken.IsRevoked = true;

            _refreshTokenRepository.Update(existingRefreshToken);

            await _unitOfWork.SaveChangesAsync(personId: null, cancellationToken);
        }

    }
}
