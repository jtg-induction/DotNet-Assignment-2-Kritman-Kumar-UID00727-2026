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
        public const string CityRequired = "City is required.";
        public const string CountryRequired = "Country is required.";
        public const string AddressLine1Required = "Address line 1 is required";
        public const string AddressLine2Required = "Address line 2 is required";
        public const string RestaurantNameRequired = "Restaurant Name is required";
        public const string DescriptionRequired = "Description is required";

        public const string NameMaxLength = "Name cannot exceed the maximum allowed length.";
        public const string EmailMaxLength = "Email cannot exceed the maximum allowed length.";
        public const string PasswordMinLength = "Password must be at least {1} characters long.";
        public const string PasswordMaxLength = "Password cannot exceed the maximum allowed length.";
        public const string MobileNumberMaxLength = "Mobile number cannot exceed the maximum allowed length."; 
        public const string ListMinLength = "The list must contain at least {0} item";

    }
}
