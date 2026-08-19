namespace RestaurantServer.Constants
{
    public static class ValidationConstants
    {
        public const string DecimalMax = "79228162514264337593543950335";
        public const int NameMaxLength = 150;
        public const int EmailMaxLength = 150;
        public const int PasswordHashMaxLength = 500;
        public const int MobileNumberMaxLength = 20;
        public const int AddressMaxLength = 200;
        public const int DescriptionMaxLength = 500;
        public const int CityMaxLength = 100;
        public const int CountryMaxLength = 100;
        public const int PostalCodeMaxLength = 20;
        public const int PasswordMinLength = 6;
        public const int PasswordMaxLength = 100;
        public const int RefreshTokenMaxLength = 500;
        public const string EmailRegex = @"^[A-Za-z0-9]+([._-][A-Za-z0-9]+)*@[A-Za-z0-9]+([.-][A-Za-z0-9]+)*\.[A-Za-z]{2,}$";
        public const string PasswordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
        public const string MobileNumberRgex = @"^[6-9]\d{9}$";
    }
}
