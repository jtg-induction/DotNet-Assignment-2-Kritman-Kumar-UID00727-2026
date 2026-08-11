using RestaurantServer.Helpers.Interfaces;

namespace RestaurantServer.Helpers.Implementations
{
    public class PasswordHasher : IPasswordHasher
    {

        /// <summary>
        /// Generates a secure hash for the specified password.
        /// </summary>
        /// <param name="password">
        /// The plain-text password to hash.
        /// </param>
        /// <returns>
        /// The hashed password.
        /// </returns>
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Verifies whether the specified password matches the stored password hash.
        /// </summary>
        /// <param name="password">
        /// The plain-text password to verify.
        /// </param>
        /// <param name="passwordHash">
        /// The stored hash to verify the password against.
        /// </param>
        /// <returns>
        /// <c>true</c> if the password matches the hash; otherwise, <c>false</c>.
        /// </returns>
        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
