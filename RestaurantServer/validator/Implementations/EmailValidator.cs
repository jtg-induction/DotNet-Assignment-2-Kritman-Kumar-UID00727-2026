using RestaurantServer.Constants;
using RestaurantServer.Validator.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace RestaurantServer.Validator.Implementations
{
    public class EmailValidator : IEmailValidator
    {
        /// <summary>
        /// Validates the email using Regex.
        /// </summary>
        /// <param name="email">owner email.</param>
        /// <returns>return true if email it valid else false.</returns>
        public bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                    !System.Text.RegularExpressions.Regex.IsMatch(email.Trim(), Constants.Regex.EmailRegex))
            {
                return false;
            }

            return true;
        }
    }
}
