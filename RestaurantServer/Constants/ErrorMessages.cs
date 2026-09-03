namespace RestaurantServer.Constants
{
    public static class ErrorMessages
    {
        public const string InternalServerError = "An unexpected error occurred. Please try again later.";
        public const string Unauthorized = "You are not authorized to perform this action.";
        public const string NotFound = "The requested resource was not found.";
        public const string ValidationFailed = "Validation failed.";
        public const string ListRequiredInvalidType = "{0} can only be applied to collection properties.";
        public const string InvalidEmail = "The email '{0}' is invalid.";
        public const string InvalidCredentials = "Invalid email or password.";
        public const string InvalidRefreshToken = "Invalid refresh token.";
        public const string EmailAlreadyExists = "An account with this email already exists.";
        public const string MobileNumberAlreadyExists = "Mobile number already exists."; 
        public const string UserInactive = "Your account has been deactivated.";
        public const string InvalidMobileNumber = "Mobile number is invalid.";
        public const string NotAuthorized = "You are not authorized to access this resource.";
        public const string InvalidPasswordFormat = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.";
        public const string EmptyRequest = "Request body cannot be empty.";
        public const string RestaurantNotExists = "Restaurant does not exist.";
        public const string RestaurantNotavailable = "Restaurant is not available.";
        public const string OwnerRelationshipAlreadyExists = "User is already an owner of this restaurant.";
        public const string InvalidRestaurantOwner = "Admin users cannot be assigned as restaurant owners.";
        public const string DuplicateOwnerEmail = "Duplicate owner email found in the request.";
        public const string RestaurantMobileNumberAlreadyExists = "A restaurant with this mobile number already exists.";
        public const string UserNotFound = "The user '{0}' was not found.";
        public const string UserDeactivated = "The user '{0}' is deactivated.";
        public const string UserInvalidRole = "The user '{0}' does not have the required role.";

    }
}
