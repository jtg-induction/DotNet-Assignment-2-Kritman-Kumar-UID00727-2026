using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Implementations;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantServer.Tests.Repositories
{
    [TestClass]
    public class RepositoryTests
    {
        private ApplicationDbContext _context;
        private Repository<User> _repository;
        private UsersRepository _userReposeroty;
        private RefreshTokenRepository _refreshTokenRepository;
        private UnitOfWork _unitOfWork;

        [TestInitialize]
        public void Setup()
        {
            _context = new ApplicationDbContext();

            _repository = new Repository<User>(_context);
            _userReposeroty = new UsersRepository(_context);
            _refreshTokenRepository = new RefreshTokenRepository(_context);
            _unitOfWork = new UnitOfWork(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetByIdAsync_ExistingUser_ReturnsUser()
        {
            var user = new User
            {
                Name = "Repository Test User",
                Email = "repository-getbyid@test.com",
                PasswordHash = "hashed-password",
                IsActive = true,
                Balance = 1000m,
                Role = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var result = await _repository.GetByIdAsync(
                    user.Id,
                    CancellationToken.None);

                Assert.IsNotNull(result);
                Assert.AreEqual(user.Id, result.Id);
                Assert.AreEqual(
                    "repository-getbyid@test.com",
                    result.Email);
            }
            finally
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task GetByIdAsync_UserDoesNotExist_ReturnsNull()
        {
            var result = await _repository.GetByIdAsync(
                long.MaxValue,
                CancellationToken.None);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task AddAsync_NewUser_AddsUserToContext()
        {
            var user = new User
            {
                Name = "Repository Add Test",
                Email = "repository-add@test.com",
                PasswordHash = "hashed-password",
                IsActive = true,
                Balance = 1000m,
                Role = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                await _repository.Add(user);
                await _context.SaveChangesAsync();

                var result = await _context.Users
                    .FindAsync(user.Id);

                Assert.IsNotNull(result);
                Assert.AreEqual(
                    "repository-add@test.com",
                    result.Email);
            }
            finally
            {
                var userToRemove = await _context.Users
                    .FindAsync(user.Id);

                if (userToRemove != null)
                {
                    _context.Users.Remove(userToRemove);
                    await _context.SaveChangesAsync();
                }
            }
        }

        [TestMethod]
        public async Task Update_ExistingUser_UpdatesUser()
        {
            var user = new User
            {
                Name = "Before Update",
                Email = "repository-update@test.com",
                PasswordHash = "hashed-password",
                IsActive = true,
                Balance = 1000m,
                Role = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var userId = user.Id;

                _context.Entry(user).State =
                    System.Data.Entity.EntityState.Detached;

                user.Name = "After Update";

                _repository.Update(user);

                await _context.SaveChangesAsync();

                var result = await _context.Users
                    .FindAsync(userId);

                Assert.IsNotNull(result);
                Assert.AreEqual(
                    "After Update",
                    result.Name);
            }
            finally
            {
                var userToRemove = await _context.Users
                    .FindAsync(user.Id);

                if (userToRemove != null)
                {
                    _context.Users.Remove(userToRemove);
                    await _context.SaveChangesAsync();
                }
            }
        }

        [TestMethod]
        public async Task Remove_ExistingUser_RemovesUser()
        {
            var user = new User
            {
                Name = "Repository Remove Test",
                Email = "repository-remove@test.com",
                PasswordHash = "hashed-password",
                IsActive = true,
                Balance = 1000m,
                Role = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _repository.Remove(user);
            await _context.SaveChangesAsync();

            var result = await _context.Users
                .FindAsync(user.Id);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetUserByEmailAsync_ExistingEmail_ReturnsUser()
        {
            var user = new User
            {
                Name = "Auth Repository Test",
                Email = "repository-auth@test.com",
                PasswordHash = "hashed-password",
                IsActive = true,
                Balance = 1000m,
                Role = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

            var result =
                await _userReposeroty.GetUserByEmailAsync(
                    "repository-auth@test.com");

                Assert.IsNotNull(result);
                Assert.AreEqual(user.Id, result.Id);
                Assert.AreEqual(
                    "repository-auth@test.com",
                    result.Email);
            }
            finally
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task GetUserByEmailAsync_EmailDoesNotExist_ReturnsNull()
        {
            var result =
                await _userReposeroty.GetUserByEmailAsync(
                    "email-does-not-exist@test.com");

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByTokenAsync_ExistingToken_ReturnsRefreshToken()
        {
            var user = new User
            {
                Name = "Refresh Token Test User",
                Email = "repository-token@test.com",
                PasswordHash = "hashed-password",
                IsActive = true,
                Balance = 1000m,
                Role = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var refreshToken = new RefreshToken(user.Id)
                {
                    Token = "repository-test-refresh-token"
                };

                _context.RefreshTokens.Add(refreshToken);
                await _context.SaveChangesAsync();

                var result =
                    await _refreshTokenRepository.GetByTokenAsync(
                        "repository-test-refresh-token",
                        CancellationToken.None);

                Assert.IsNotNull(result);
                Assert.AreEqual(
                    refreshToken.Id,
                    result.Id);
                Assert.AreEqual(
                    "repository-test-refresh-token",
                    result.Token);
            }
            finally
            {
                var tokenToRemove = await _context.RefreshTokens
                    .FirstOrDefaultAsync(
                        token => token.Token ==
                            "repository-test-refresh-token");

                if (tokenToRemove != null)
                {
                    _context.RefreshTokens.Remove(tokenToRemove);
                }

                var userToRemove = await _context.Users
                    .FindAsync(user.Id);

                if (userToRemove != null)
                {
                    _context.Users.Remove(userToRemove);
                }

                await _context.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task GetByTokenAsync_TokenDoesNotExist_ReturnsNull()
        {
            var result =
                await _refreshTokenRepository.GetByTokenAsync(
                    "token-does-not-exist",
                    CancellationToken.None);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task RevokeAllByUserIdAsync_ActiveTokens_RevokesAllTokens()
        {
            var user = new User
            {
                Name = "Revoke Token Test User",
                Email = "repository-revoke@test.com",
                PasswordHash = "hashed-password",
                IsActive = true,
                Balance = 1000m,
                Role = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var activeToken1 = new RefreshToken(user.Id)
                {
                    Token = "active-token-1",
                    IsRevoked = false
                };

                var activeToken2 = new RefreshToken(user.Id)
                {
                    Token = "active-token-2",
                    IsRevoked = false
                };

                _context.RefreshTokens.Add(activeToken1);
                _context.RefreshTokens.Add(activeToken2);

                await _context.SaveChangesAsync();

                await _refreshTokenRepository
                    .RevokeAllByUserIdAsync(
                        user.Id,
                        CancellationToken.None);

                Assert.IsTrue(activeToken1.IsRevoked);
                Assert.IsTrue(activeToken2.IsRevoked);

                Assert.IsTrue(
                    activeToken1.UpdatedAt > DateTime.MinValue);

                Assert.IsTrue(
                    activeToken2.UpdatedAt > DateTime.MinValue);
            }
            finally
            {
                var tokens = await _context.RefreshTokens
                    .Where(token =>
                        token.UserId == user.Id)
                    .ToListAsync();

                _context.RefreshTokens.RemoveRange(tokens);

                var userToRemove = await _context.Users
                    .FindAsync(user.Id);

                if (userToRemove != null)
                {
                    _context.Users.Remove(userToRemove);
                }

                await _context.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task RevokeAllByUserIdAsync_AlreadyRevokedToken_DoesNotChangeRevokedState()
        {
            var user = new User
            {
                Name = "Already Revoked Test User",
                Email = "repository-already-revoked@test.com",
                PasswordHash = "hashed-password",
                IsActive = true,
                Balance = 1000m,
                Role = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var revokedToken = new RefreshToken(user.Id)
                {
                    Token = "already-revoked-token",
                    IsRevoked = true
                };

                _context.RefreshTokens.Add(revokedToken);
                await _context.SaveChangesAsync();

                var originalUpdatedAt =
                    revokedToken.UpdatedAt;

                await _refreshTokenRepository
                    .RevokeAllByUserIdAsync(
                        user.Id,
                        CancellationToken.None);

                Assert.IsTrue(revokedToken.IsRevoked);

                Assert.AreEqual(
                    originalUpdatedAt,
                    revokedToken.UpdatedAt);
            }
            finally
            {
                var tokenToRemove =
                    await _context.RefreshTokens
                        .FirstOrDefaultAsync(
                            token => token.Token ==
                                "already-revoked-token");

                if (tokenToRemove != null)
                {
                    _context.RefreshTokens.Remove(tokenToRemove);
                }

                var userToRemove = await _context.Users
                    .FindAsync(user.Id);

                if (userToRemove != null)
                {
                    _context.Users.Remove(userToRemove);
                }

                await _context.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task RevokeAllByUserIdAsync_DifferentUser_DoesNotRevokeToken()
        {
            var user1 = new User
            {
                Name = "User One",
                Email = "repository-user-one@test.com",
                PasswordHash = "hashed-password",
                IsActive = true,
                Balance = 1000m,
                Role = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var user2 = new User
            {
                Name = "User Two",
                Email = "repository-user-two@test.com",
                PasswordHash = "hashed-password",
                IsActive = true,
                Balance = 1000m,
                Role = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                _context.Users.Add(user1);
                _context.Users.Add(user2);

                await _context.SaveChangesAsync();

                var token = new RefreshToken(user2.Id)
                {
                    Token = "different-user-token",
                    IsRevoked = false
                };

                _context.RefreshTokens.Add(token);
                await _context.SaveChangesAsync();

                await _refreshTokenRepository
                    .RevokeAllByUserIdAsync(
                        user1.Id,
                        CancellationToken.None);

                Assert.IsFalse(token.IsRevoked);
            }
            finally
            {
                var tokenToRemove =
                    await _context.RefreshTokens
                        .FirstOrDefaultAsync(
                            refreshToken =>
                                refreshToken.Token ==
                                "different-user-token");

                if (tokenToRemove != null)
                {
                    _context.RefreshTokens.Remove(tokenToRemove);
                }

                var userOneToRemove =
                    await _context.Users.FindAsync(user1.Id);

                if (userOneToRemove != null)
                {
                    _context.Users.Remove(userOneToRemove);
                }

                var userTwoToRemove =
                    await _context.Users.FindAsync(user2.Id);

                if (userTwoToRemove != null)
                {
                    _context.Users.Remove(userTwoToRemove);
                }

                await _context.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task SaveChangesAsync_ChangesArePersisted()
        {
            var user = new User
            {
                Name = "Unit Of Work Test",
                Email = "repository-unitofwork@test.com",
                PasswordHash = "hashed-password",
                IsActive = true,
                Balance = 1000m,
                Role = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                _context.Users.Add(user);

                await _unitOfWork.SaveChangesAsync(
                    CancellationToken.None);

                var result = await _context.Users
                    .FindAsync(user.Id);

                Assert.IsNotNull(result);
                Assert.AreEqual(
                    "repository-unitofwork@test.com",
                    result.Email);
            }
            finally
            {
                var userToRemove = await _context.Users
                    .FindAsync(user.Id);

                if (userToRemove != null)
                {
                    _context.Users.Remove(userToRemove);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
