using RestaurantServer.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantServer.ModelStateValidator
{
    public class ListRequiredAttribute : ValidationAttribute
    {
        private readonly int _minLength;

        public ListRequiredAttribute(int minLength)
        {
            _minLength = minLength;
        }

        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext)
        {
            if (value is List<string> emails)
            {
                if (emails.Count < _minLength)
                {
                    return new ValidationResult(
                        string.Format(ValidationMessages.ListMinLength, _minLength)
                    );
                }
            }

            return ValidationResult.Success;
        }
    }
}
