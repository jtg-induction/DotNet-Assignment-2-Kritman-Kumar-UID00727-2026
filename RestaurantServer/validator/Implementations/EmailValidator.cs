using RestaurantServer.Constants;
using System.ComponentModel.DataAnnotations;

namespace RestaurantServer.Validator.Implementations
{
    public class EmailValidator
    {

        /// <summary>
        /// Validates the email using Regex.
        /// </summary>
        /// <param name="email">owner email</param>
        /// <exception cref="ValidationException">throws exception when email is empty or when email is invlaid.</exception>
        public void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                    !System.Text.RegularExpressions.Regex.IsMatch(email.Trim(), Constants.Regex.EmailRegex))
            {
                throw new ValidationException(ValidationMessages.InvalidEmail);
            }
        }
    }
}
