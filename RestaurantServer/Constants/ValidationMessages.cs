using System.Reflection.Emit;
using System.Runtime.CompilerServices;

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
        public const string QuantityRequired = "Quantity id required.";
        public const string ItemIdRequired = "ItemId is required.";
        public const string RestaurantOwnerRequired = "You are not authorized to update this order.";
        public const string ReportRequestRequired = "Report request cannot be null.";

        public const string NameMaxLength = "Name cannot exceed the maximum allowed length.";
        public const string EmailMaxLength = "Email cannot exceed the maximum allowed length.";
        public const string PasswordMinLength = "Password must be at least {1} characters long.";
        public const string PasswordMaxLength = "Password cannot exceed the maximum allowed length.";
        public const string MobileNumberMaxLength = "Mobile number cannot exceed the maximum allowed length.";
        public const string OnboardRestaurantOwnerEmailsMinLength = "Owners emails must include 1 email.";
        public const string InvalidTopItemsCount = "TopItems must be greater than 0 and TopItems cannot be greater than 100.";
        public const string InvalidTopPairsCount = "TopPairs must be greater than 0 and TopPairs cannot be greater than 100.";

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
        public const string OwnerRelationshipAlreadyExists = "User is already an owner of this restaurant.";
        public const string InvalidRestaurantOwner = "Admin users cannot be assigned as restaurant owners.";
        public const string DuplicateOwnerEmail = "Duplicate owner email found in the request.";
        public const string RestaurantMobileNumberAlreadyExists = "A restaurant with this mobile number already exists.";
        public const string InvalidPageNumber = "Page number must be greater than or equal to 1.";
        public const string InvalidPageSize = "Page size must be greater than or equal to 1.";
        public const string OrderItemsRequired = "Order must contain at least one item.";
        public const string InvalidItemId = "Item ID is invalid.";
        public const string InvalidQuantity = "Quantity must be at least 1.";
        public const string InsufficientStock = "Insufficient stock for item.";
        public const string InsufficientBalance = "Insufficient balance to place order.";
        public const string ItemNotFound = "Item not found.";
        public const string ItemNotAvailable = "Item is not available.";
        public const string ItemDoesNotBelongToRestaurant = "Item does not belong to the specified restaurant.";
        public const string InvalidRole = "User role is not allowed to place an order.";
        public const string OrderNotFound = "Order not found.";
        public const string InvalidOrderId = "Order ID is invalid.";
        public const string OrderCannotBeCancelled = "Order can only be cancelled when it is in Placed or Accepted status.";
        public const string InvalidOrderStatus = "Invalid order status.";
        public const string InvalidOrderStatusTransition = "Invalid order status transition.";
        public const string OrderCannotBeCancelledByOwner = "Order cancellation is handled by the customer.";
        public const string InvalidTopItems = "Top items must be between 1 and 100.";
        public const string InvalidDateRange = "StartDate cannot be later than EndDate.";
        public const string InvalidExcludeItemIds = "ExcludeItemIds must contain only valid item IDs.";
        public const string InvalidRestaurantId = "RestaurantId must be greater than 0.";
        public const string InvalidPath = "Could not map the report path: ";
        public const string ReportNotFound = "The Telerik report file was not found.";
        public const string TelerikFileLengthZero = "The Telerik report file exists but its file size is 0 bytes. Open the report in the Telerik Report Designer and save it.";
        public const string ReportIsNull = "Telerik could not load the report.";
        public const string RenderReportisNull = "Telerik RenderReport returned a null result.";
        public const string DocumentBytesisNull = "Telerik rendered the report, but DocumentBytes is null.";
        public const string EmptyPdf = "Telerik generated an empty PDF.";
    }
}
