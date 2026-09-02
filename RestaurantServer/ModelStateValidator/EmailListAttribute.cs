using RestaurantServer.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace RestaurantServer.ModelStateValidator
{
    public class EmailListAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext)
        {
            if (value is List<string> emails)
            {
                var invalidEmails = emails
                    .Where(email => string.IsNullOrWhiteSpace(email) ||
                    !System.Text.RegularExpressions.Regex.IsMatch(email.Trim(), Constants.Regex.EmailRegex))
                    .ToList();

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
