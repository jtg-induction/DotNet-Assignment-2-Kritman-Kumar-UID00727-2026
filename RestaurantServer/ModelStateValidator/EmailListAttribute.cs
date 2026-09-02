using RestaurantServer.Constants;
using RestaurantServer.Validator.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace RestaurantServer.ModelStateValidator
{
    /// <summary>
    /// Provides validation for a list of email addresses.
    /// </summary>
    public class EmailListAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validates that all email addresses in the list are non-empty
        /// and match the configured email format.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context for the validation operation.</param>
        /// <returns>
        /// A <see cref="ValidationResult"/> containing the invalid email addresses
        /// if validation fails; otherwise, <see cref="ValidationResult.Success"/>.
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            IEmailValidator emailValidator = value as IEmailValidator;

            if (value is List<string> emails)
            {
                var invalidEmails = emails
                    .Where(email => !emailValidator.ValidateEmail(email)).ToList();

                if (invalidEmails.Any())
                {
                    var invalidEmailList = string.Join(", ", invalidEmails);

                    return new ValidationResult(
                        string.Format(ValidationMessages.InvalidEmail, invalidEmailList)
                    );
                }
            }

            return ValidationResult.Success;
        }
    }
} 