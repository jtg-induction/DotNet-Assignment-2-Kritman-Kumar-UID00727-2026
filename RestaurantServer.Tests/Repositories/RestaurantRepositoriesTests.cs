using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestaurantServer.Models;
using RestaurantServer.Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantServer.Tests.Repositories
{
    [TestClass]
    public class RestaurantRepositoryTests
    {
        private ApplicationDbContext _context;
        private RestaurantRepository _restaurantRepository;

        [TestInitialize]
        public void Setup()
        {
            _context = new ApplicationDbContext();
            _restaurantRepository = new RestaurantRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task ExistsByMobileNumberAsync_ExistingActiveRestaurant_ReturnsTrue()
        {
            var user = CreateUser("Restaurant Mobile Test User");
            Restaurant restaurant = null;

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                restaurant = CreateRestaurant(
                    "Mobile Test Restaurant",
                    "9876500001",
                    user.Id,
                    false);

                _context.Restaurants.Add(restaurant);
                await _context.SaveChangesAsync();

                var result =
                    await _restaurantRepository.ExistsByMobileNumberAsync(
                        restaurant.MobileNumber);

                Assert.IsTrue(result);
            }
            finally
            {
                await DeleteRestaurantAndUserAsync(restaurant, user);
            }
        }

        [TestMethod]
        public async Task ExistsByMobileNumberAsync_MobileNumberDoesNotExist_ReturnsFalse()
        {
            var mobileNumber =
                $"9{Guid.NewGuid():N}".Substring(0, 10);

            var result =
                await _restaurantRepository.ExistsByMobileNumberAsync(
                    mobileNumber);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ExistsByMobileNumberAsync_DeletedRestaurant_ReturnsFalse()
        {
            var user = CreateUser("Deleted Restaurant Test User");
            Restaurant restaurant = null;

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                restaurant = CreateRestaurant(
                    "Deleted Restaurant",
                    "9876500002",
                    user.Id,
                    true);

                _context.Restaurants.Add(restaurant);
                await _context.SaveChangesAsync();

                var result =
                    await _restaurantRepository.ExistsByMobileNumberAsync(
                        restaurant.MobileNumber);

                Assert.IsFalse(result);
            }
            finally
            {
                await DeleteRestaurantAndUserAsync(restaurant, user);
            }
        }

        [TestMethod]
        public async Task ExistsByMobileNumberAsync_ActiveRestaurantWithDifferentMobileNumber_ReturnsFalse()
        {
            var user = CreateUser("Different Mobile Test User");
            Restaurant restaurant = null;

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                restaurant = CreateRestaurant(
                    "Different Mobile Restaurant",
                    "9876500003",
                    user.Id,
                    false);

                _context.Restaurants.Add(restaurant);
                await _context.SaveChangesAsync();

                var result =
                    await _restaurantRepository.ExistsByMobileNumberAsync(
                        "9876500004");

                Assert.IsFalse(result);
            }
            finally
            {
                await DeleteRestaurantAndUserAsync(restaurant, user);
            }
        }

        private User CreateUser(string name)
        {
            return new User
            {
                Name = name,
                Email = $"restaurant-test-{Guid.NewGuid():N}@test.com",
                PasswordHash = "hashed-password",
                IsActive = true,
                Balance = 1000m,
                Role = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        private Restaurant CreateRestaurant(
            string restaurantName,
            string mobileNumber,
            long userId,
            bool isDeleted)
        {
            return new Restaurant
            {
                RestaurantName = restaurantName,
                Description = "Restaurant repository test",
                MobileNumber = mobileNumber,
                AddressLine1 = "123 Main Street",
                AddressLine2 = "Near Mall",
                City = "Dehradun",
                Country = "India",
                PostalCode = "248001",
                CreatedBy = userId,
                UpdatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = isDeleted
            };
        }

        private async Task DeleteRestaurantAndUserAsync(
            Restaurant restaurant,
            User user)
        {
            try
            {
                if (restaurant != null && restaurant.Id > 0)
                {
                    var restaurantToRemove =
                        await _context.Restaurants.FindAsync(restaurant.Id);

                    if (restaurantToRemove != null)
                    {
                        _context.Restaurants.Remove(restaurantToRemove);
                    }
                }

                if (user != null && user.Id > 0)
                {
                    var userToRemove =
                        await _context.Users.FindAsync(user.Id);

                    if (userToRemove != null)
                    {
                        _context.Users.Remove(userToRemove);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch
            {
            }
        }
    }

    [TestClass]
    public class RestaurantOwnerRepositoryTests
    {
        private ApplicationDbContext _context;
        private RestaurantOwnerRepository _restaurantOwnerRepository;

        [TestInitialize]
        public void Setup()
        {
            _context = new ApplicationDbContext();

            _restaurantOwnerRepository =
                new RestaurantOwnerRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetOwnersByRestaurantAndUserIdsAsync_ExistingRelationship_ReturnsOwner()
        {
            var user = CreateUser("Restaurant Owner Test User");
            Restaurant restaurant = null;
            RestaurantOwner restaurantOwner = null;

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                restaurant = CreateRestaurant(
                    "Owner Test Restaurant",
                    "9876500010",
                    user.Id);

                _context.Restaurants.Add(restaurant);
                await _context.SaveChangesAsync();

                restaurantOwner = new RestaurantOwner
                {
                    RestaurantId = restaurant.Id,
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.RestaurantOwners.Add(restaurantOwner);
                await _context.SaveChangesAsync();

                var result =
                    await _restaurantOwnerRepository
                        .GetOwnersByRestaurantAndUserIdsAsync(
                            restaurant.Id,
                            new List<long> { user.Id });

                Assert.IsNotNull(result);
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(restaurantOwner.Id, result[0].Id);
                Assert.AreEqual(restaurant.Id, result[0].RestaurantId);
                Assert.AreEqual(user.Id, result[0].UserId);
            }
            finally
            {
                await DeleteRestaurantOwnerDataAsync(
                    restaurantOwner,
                    restaurant,
                    user);
            }
        }

        [TestMethod]
        public async Task GetOwnersByRestaurantAndUserIdsAsync_RelationshipDoesNotExist_ReturnsEmptyList()
        {
            var result =
                await _restaurantOwnerRepository
                    .GetOwnersByRestaurantAndUserIdsAsync(
                        long.MaxValue,
                        new List<long> { long.MaxValue });

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetOwnersByRestaurantAndUserIdsAsync_SameRestaurantDifferentUser_ReturnsEmptyList()
        {
            var user1 = CreateUser("Restaurant Owner User One");
            var user2 = CreateUser("Restaurant Owner User Two");

            Restaurant restaurant = null;
            RestaurantOwner restaurantOwner = null;

            try
            {
                _context.Users.Add(user1);
                _context.Users.Add(user2);

                await _context.SaveChangesAsync();

                restaurant = CreateRestaurant(
                    "Different User Restaurant",
                    "9876500011",
                    user1.Id);

                _context.Restaurants.Add(restaurant);
                await _context.SaveChangesAsync();

                restaurantOwner = new RestaurantOwner
                {
                    RestaurantId = restaurant.Id,
                    UserId = user1.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.RestaurantOwners.Add(restaurantOwner);
                await _context.SaveChangesAsync();

                var result =
                    await _restaurantOwnerRepository
                        .GetOwnersByRestaurantAndUserIdsAsync(
                            restaurant.Id,
                            new List<long> { user2.Id });

                Assert.IsNotNull(result);
                Assert.AreEqual(0, result.Count);
            }
            finally
            {
                await DeleteRestaurantOwnerDataAsync(
                    restaurantOwner,
                    restaurant,
                    user1,
                    user2);
            }
        }

        [TestMethod]
        public async Task GetOwnersByRestaurantAndUserIdsAsync_DifferentRestaurantSameUser_ReturnsEmptyList()
        {
            var user = CreateUser("Same User Test");

            Restaurant restaurant1 = null;
            Restaurant restaurant2 = null;
            RestaurantOwner restaurantOwner = null;

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                restaurant1 = CreateRestaurant(
                    "Restaurant One",
                    "9876500012",
                    user.Id);

                restaurant2 = CreateRestaurant(
                    "Restaurant Two",
                    "9876500013",
                    user.Id);

                _context.Restaurants.Add(restaurant1);
                _context.Restaurants.Add(restaurant2);

                await _context.SaveChangesAsync();

                restaurantOwner = new RestaurantOwner
                {
                    RestaurantId = restaurant1.Id,
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.RestaurantOwners.Add(restaurantOwner);
                await _context.SaveChangesAsync();

                var result =
                    await _restaurantOwnerRepository
                        .GetOwnersByRestaurantAndUserIdsAsync(
                            restaurant2.Id,
                            new List<long> { user.Id });

                Assert.IsNotNull(result);
                Assert.AreEqual(0, result.Count);
            }
            finally
            {
                await DeleteRestaurantOwnerDataAsync(
                    restaurantOwner,
                    restaurant1,
                    user,
                    restaurant2);
            }
        }

        [TestMethod]
        public async Task GetOwnersByRestaurantAndUserIdsAsync_EmptyUserIds_ReturnsEmptyList()
        {
            var result =
                await _restaurantOwnerRepository
                    .GetOwnersByRestaurantAndUserIdsAsync(
                        1,
                        new List<long>());

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        private User CreateUser(string name)
        {
            return new User
            {
                Name = name,
                Email =
                    $"restaurant-owner-test-{Guid.NewGuid():N}@test.com",
                PasswordHash = "hashed-password",
                IsActive = true,
                Balance = 1000m,
                Role = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        private Restaurant CreateRestaurant(
            string restaurantName,
            string mobileNumber,
            long userId)
        {
            return new Restaurant
            {
                RestaurantName = restaurantName,
                Description = "Restaurant owner repository test",
                MobileNumber = mobileNumber,
                AddressLine1 = "123 Main Street",
                AddressLine2 = "Near Mall",
                City = "Dehradun",
                Country = "India",
                PostalCode = "248001",
                CreatedBy = userId,
                UpdatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
        }

        private async Task DeleteRestaurantOwnerDataAsync(
            RestaurantOwner restaurantOwner,
            Restaurant restaurant,
            User user1,
            User user2 = null,
            User user3 = null)
        {
            try
            {
                if (restaurantOwner != null &&
                    restaurantOwner.Id > 0)
                {
                    var ownerToRemove =
                        await _context.RestaurantOwners
                            .FindAsync(restaurantOwner.Id);

                    if (ownerToRemove != null)
                    {
                        _context.RestaurantOwners.Remove(ownerToRemove);
                    }
                }

                if (restaurant != null &&
                    restaurant.Id > 0)
                {
                    var restaurantToRemove =
                        await _context.Restaurants
                            .FindAsync(restaurant.Id);

                    if (restaurantToRemove != null)
                    {
                        _context.Restaurants.Remove(restaurantToRemove);
                    }
                }

                if (user1 != null &&
                    user1.Id > 0)
                {
                    var userToRemove =
                        await _context.Users.FindAsync(user1.Id);

                    if (userToRemove != null)
                    {
                        _context.Users.Remove(userToRemove);
                    }
                }

                if (user2 != null &&
                    user2.Id > 0)
                {
                    var userToRemove =
                        await _context.Users.FindAsync(user2.Id);

                    if (userToRemove != null)
                    {
                        _context.Users.Remove(userToRemove);
                    }
                }

                if (user3 != null &&
                    user3.Id > 0)
                {
                    var userToRemove =
                        await _context.Users.FindAsync(user3.Id);

                    if (userToRemove != null)
                    {
                        _context.Users.Remove(userToRemove);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch
            {
            }
        }

        private async Task DeleteRestaurantOwnerDataAsync(
            RestaurantOwner restaurantOwner,
            Restaurant restaurant1,
            User user,
            Restaurant restaurant2)
        {
            try
            {
                if (restaurantOwner != null &&
                    restaurantOwner.Id > 0)
                {
                    var ownerToRemove =
                        await _context.RestaurantOwners
                            .FindAsync(restaurantOwner.Id);

                    if (ownerToRemove != null)
                    {
                        _context.RestaurantOwners.Remove(ownerToRemove);
                    }
                }

                if (restaurant1 != null &&
                    restaurant1.Id > 0)
                {
                    var restaurantToRemove =
                        await _context.Restaurants
                            .FindAsync(restaurant1.Id);

                    if (restaurantToRemove != null)
                    {
                        _context.Restaurants.Remove(restaurantToRemove);
                    }
                }

                if (restaurant2 != null &&
                    restaurant2.Id > 0)
                {
                    var restaurantToRemove =
                        await _context.Restaurants
                            .FindAsync(restaurant2.Id);

                    if (restaurantToRemove != null)
                    {
                        _context.Restaurants.Remove(restaurantToRemove);
                    }
                }

                if (user != null &&
                    user.Id > 0)
                {
                    var userToRemove =
                        await _context.Users.FindAsync(user.Id);

                    if (userToRemove != null)
                    {
                        _context.Users.Remove(userToRemove);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch
            {
            }
        }
    }
}
