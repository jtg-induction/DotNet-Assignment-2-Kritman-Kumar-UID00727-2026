using System;

namespace RestaurantServer.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs when application input or business
    /// validation fails.
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class
        /// with the specified validation error message.
        /// </summary>
        /// <param name="message">
        /// The message that describes the validation error.
        /// </param>
        public ValidationException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class
        /// with the specified validation error message and a reference to the
        /// exception that caused the validation failure.
        /// </summary>
        /// <param name="message">
        /// The message that describes the validation error.
        /// </param>
        /// <param name="innerException">
        /// The exception that caused the current validation exception.
        /// </param>
        public ValidationException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
