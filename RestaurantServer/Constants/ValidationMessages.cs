namespace RestaurantServer.Constants
{
    public static class ValidationMessages
    {
        public const string NameRequired = "Name is required.";
        public const string EmailRequired = "Email is required.";
        public const string PasswordRequired = "Password is required.";
        public const string DeviceIdRequired = "Device ID is required.";
        public const string RefreshTokenRequired = "Refresh token is required.";
        public const string MobileNumberRequired = "Mobile number is required.";
        public const string AuthenticationRequired = "Authentication is required.";
        public const string PostalCodeRequired = "Postal Code is reqired.";
        public const string CityRequired = "City name is required.";
        public const string CountryRequired = "Country name is required.";
        public const string AddressLine1Required = "Address line 1 is required";
        public const string AddressLine2Required = "Address line 2 is required";
        public const string RestaurantNameRequired = "Mobile number is required";
        public const string DescriptionRequired = "Description is required";

        public const string NameMaxLength = "Name cannot exceed the maximum allowed length.";
        public const string EmailMaxLength = "Email cannot exceed the maximum allowed length.";
        public const string PasswordMinLength = "Password must be at least {1} characters long.";
        public const string PasswordMaxLength = "Password cannot exceed the maximum allowed length.";
        public const string MobileNumberMaxLength = "Mobile number cannot exceed the maximum allowed length.";
        public const string OnboardRestaurantOwnerEmailsMinLength = "Owners emails must include {1} email.";


        public const string InvalidEmail = "Please enter a valid email address.";
        public const string EmailAlreadyExists = "An account with this email already exists.";
        public const string MobileNumberAlreadyExists = "Mobile number already exists.";
        public const string InvalidCredentials = "Invalid email or password.";
        public const string UserNotFound = "User not found.";
        public const string UserInactive = "Your account has been deactivated.";
        public const string InvalidRefreshToken = "Invalid refresh token.";
        public const string InvalidMobileNumber = "Mobile number is invalid.";
        public const string NotAuthorized = "You are not authorized to access this resource.";
        public const string InvalidPasswordFormat = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.";
        public const string EmptyRequest = "Request body cannot be empty.";
        public const string RestaurantNotExists = "Restaurant does not exist.";
        public const string RestaurantNotavailable = "Restaurant is not available.";
        public const string AlreadyOwner = "User is already an owner of this restaurant.";
        public const string InvalidRestaurantOwner = "Admin users cannot be assigned as restaurant owners.";
        public const string DuplicateOwnerEmail = "Duplicate owner email found in the request.";
        public const string RestaurantMobileNumberAlreadyExists = "A restaurant with this mobile number already exists.";
    }
}
