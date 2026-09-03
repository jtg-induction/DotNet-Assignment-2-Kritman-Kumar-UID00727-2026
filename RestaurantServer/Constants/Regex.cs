namespace RestaurantServer.Constants
{
    public static class Regex
    {
        public const string EmailRegex = @"^[A-Za-z0-9]+([._-][A-Za-z0-9]+)*@[A-Za-z0-9]+([.-][A-Za-z0-9]+)*\.[A-Za-z]{2,}$";
        public const string PasswordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
        public const string MobileNumberRgex = @"^[6-9]\d{9}$";
    }
}
