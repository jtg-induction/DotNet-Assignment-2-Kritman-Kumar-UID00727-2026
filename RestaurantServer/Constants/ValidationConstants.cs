namespace RestaurantServer.Constants
{
    public static class ValidationConstants
    {
        public const string DecimalMax = "79228162514264337593543950335";
        public const long IdMaxVal = long.MaxValue;
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
        public const int MaxQuantity = int.MaxValue;
    }
}
