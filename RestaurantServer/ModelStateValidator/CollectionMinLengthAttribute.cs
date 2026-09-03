using RestaurantServer.Constants;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace RestaurantServer.ModelStateValidator
{
    /// <summary>
    /// Validates that a collection contains at least the specified minimum number of items.
    /// </summary>
    public class CollectionMinLengthAttribute : ValidationAttribute
    {
        private readonly int _minLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionMinLengthAttribute"/> class.
        /// </summary>
        /// <param name="minLength">
        /// The minimum number of items required in the collection. Defaults to 1.
        /// </param>
        public CollectionMinLengthAttribute(int minLength = 1)
        {
            _minLength = minLength;
        }

        /// <summary>
        /// Validates that the provided value is a collection and contains
        /// at least the configured minimum number of items.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context for the validation operation.</param>
        /// <returns>
        /// A <see cref="ValidationResult"/> if the collection does not contain
        /// the required number of items; otherwise, <see cref="ValidationResult.Success"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the attribute is applied to a value that is not a collection.
        /// </exception>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var collection = value as ICollection;

            if (null == collection)
            {
                throw new ArgumentException(
                    string.Format(ErrorMessages.ListRequiredInvalidType,
                        nameof(CollectionMinLengthAttribute)), validationContext.MemberName);
            }

            if (collection.Count < _minLength)
            {
                return new ValidationResult(string.Format(ValidationMessages.ListMinLength,  _minLength));
            }

            return ValidationResult.Success;
        }
    }
}
